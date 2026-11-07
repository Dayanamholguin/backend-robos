using AppRobos.Backend.Dtos;
using AppRobos.Backend.Seguridad;
using AppRobos.Backend.Modelos;
using Microsoft.AspNetCore.Identity;

namespace AppRobos.Backend.Servicios;

// Caso de uso de autenticación: registro (RF-001), inicio de sesión (RF-002) y consulta de perfil (RF-003).
// Coordina Identity, el bloqueo escalonado, los tokens y los catálogos.
public class ServicioAutenticacion
{
    // Correo de soporte que se muestra cuando la cuenta queda deshabilitada.
    public const string CorreoSoporte = "app@pruebas.com";

    private readonly UserManager<CuentaUsuario> _usuarios;
    private readonly ServicioBloqueoCuenta _bloqueo;
    private readonly ServicioTokenJwt _tokens;
    private readonly ServicioCatalogos _catalogos;
    private readonly TimeProvider _reloj;
    private readonly ContextoSesionBaseDatos _sesionBd;

    public ServicioAutenticacion(
        UserManager<CuentaUsuario> usuarios,
        ServicioBloqueoCuenta bloqueo,
        ServicioTokenJwt tokens,
        ServicioCatalogos catalogos,
        TimeProvider reloj,
        ContextoSesionBaseDatos sesionBd)
    {
        _sesionBd = sesionBd;
        _usuarios = usuarios;
        _bloqueo = bloqueo;
        _tokens = tokens;
        _catalogos = catalogos;
        _reloj = reloj;
    }

    // Registra un ciudadano nuevo (RF-001): valida el correo y el sexo/género,
    // crea la cuenta en estado ACTIVA y le asigna el rol Ciudadano.
    public async Task<ResultadoAuth<PerfilUsuarioDto>> RegistrarAsync(RegistroUsuarioDto datos)
    {
        // El registro necesita consultar cuentas antes de que exista una sesión: modo sistema solo para esta operación
        _sesionBd.ActivarModoSistema();

        var correo = datos.Correo.Trim().ToLowerInvariant();

        // El correo no puede estar asociado a otra cuenta
        if (await _usuarios.FindByEmailAsync(correo) is not null)
        {
            return ResultadoAuth<PerfilUsuarioDto>.Sin(
                EstadoResultadoAuth.Conflicto,
                "Ya existe una cuenta con ese correo electrónico.");
        }

        // El sexo/género debe estar en la lista habilitada de la base de datos
        if (!await _catalogos.CodigoHabilitadoAsync(CatalogosAplicacion.SexoGenero, datos.SexoCodigo))
        {
            return ResultadoAuth<PerfilUsuarioDto>.Invalido(["El sexo o género seleccionado no es válido."]);
        }

        var ahora = _reloj.GetUtcNow();
        var cuenta = new CuentaUsuario
        {
            // El correo es el usuario y se guarda en minúsculas para evitar duplicados
            UserName = correo,
            Email = correo,
            PhoneNumber = datos.Telefono,
            Nombre = datos.Nombre.Trim(),
            Apellido = datos.Apellido.Trim(),
            FechaNacimiento = datos.FechaNacimiento,
            SexoCodigo = datos.SexoCodigo,
            EstadoCodigo = EstadosCuentaCodigos.Activa,
            // Activa el bloqueo en la cuenta; el escalonamiento lo controla ServicioBloqueoCuenta
            LockoutEnabled = true,
            FechaCreacion = ahora,
            FechaActualizacion = ahora
        };

        // Identity valida la complejidad de la contraseña y guarda solo su hash
        var resultado = await _usuarios.CreateAsync(cuenta, datos.Contrasena);
        if (!resultado.Succeeded)
        {
            return ResultadoAuth<PerfilUsuarioDto>.Invalido(TraductorErroresIdentity.Traducir(resultado.Errors));
        }

        // Todo ciudadano nuevo recibe el rol Ciudadano, y ningún otro rol
        await _usuarios.AddToRoleAsync(cuenta, RolesAplicacion.Ciudadano);

        return ResultadoAuth<PerfilUsuarioDto>.Exito(
            MapearPerfil(cuenta, [RolesAplicacion.Ciudadano]));
    }

    // Inicia sesión (RF-002). Revisa en orden: cuenta deshabilitada, bloqueo temporal,
    // estado inactivo y contraseña. Un fallo cuenta para el bloqueo escalonado.
    public async Task<ResultadoAuth<RespuestaLoginDto>> IniciarSesionAsync(LoginUsuarioDto datos)
    {
        // El login busca la cuenta por correo antes de conocer su identificador: modo sistema solo para esta operación
        _sesionBd.ActivarModoSistema();

        var ahora = _reloj.GetUtcNow();
        var cuenta = await _usuarios.FindByEmailAsync(datos.Correo.Trim().ToLowerInvariant());

        // No se revela si el correo existe o no
        if (cuenta is null)
        {
            return ResultadoAuth<RespuestaLoginDto>.Sin(
                EstadoResultadoAuth.CredencialesInvalidas,
                "Correo o contraseña incorrectos.");
        }

        // Una cuenta deshabilitada no puede iniciar sesión ni aunque tenga la contraseña correcta
        if (cuenta.CuentaDeshabilitada)
        {
            return MensajeDeshabilitada();
        }

        // Si el bloqueo temporal venció, la cuenta vuelve a estar activa
        _bloqueo.LiberarBloqueoVencido(cuenta, ahora);

        if (_bloqueo.EstaBloqueadaTemporalmente(cuenta, ahora))
        {
            return ResultadoAuth<RespuestaLoginDto>.Bloqueo(cuenta.LockoutEnd!.Value);
        }

        if (cuenta.EstadoCodigo == EstadosCuentaCodigos.Inactiva)
        {
            return ResultadoAuth<RespuestaLoginDto>.Sin(
                EstadoResultadoAuth.Inactiva,
                "La cuenta está inactiva.");
        }

        // Contraseña incorrecta: se registra el fallo y se aplica el bloqueo si corresponde
        if (!await _usuarios.CheckPasswordAsync(cuenta, datos.Contrasena))
        {
            var resultadoFallo = _bloqueo.RegistrarFallo(cuenta, ahora);
            await _usuarios.UpdateAsync(cuenta);

            return resultadoFallo switch
            {
                ResultadoFalloLogin.BloqueoTemporal => ResultadoAuth<RespuestaLoginDto>.Bloqueo(cuenta.LockoutEnd!.Value),
                ResultadoFalloLogin.BloqueoPermanente => MensajeDeshabilitada(),
                _ => ResultadoAuth<RespuestaLoginDto>.Sin(
                    EstadoResultadoAuth.CredencialesInvalidas,
                    "Correo o contraseña incorrectos.")
            };
        }

        // Login exitoso: se reinicia el escalonamiento y se entrega el token
        _bloqueo.LimpiarTrasLoginExitoso(cuenta);
        await _usuarios.UpdateAsync(cuenta);

        var roles = await _usuarios.GetRolesAsync(cuenta);
        var token = _tokens.CrearToken(cuenta, roles, ahora);

        return ResultadoAuth<RespuestaLoginDto>.Exito(
            new RespuestaLoginDto(token.Token, token.ExpiraEn, MapearPerfil(cuenta, roles)));
    }

    // Devuelve el perfil de la cuenta indicada (RF-003).
    public async Task<ResultadoAuth<PerfilUsuarioDto>> ObtenerPerfilAsync(Guid idCuenta)
    {
        var cuenta = await _usuarios.FindByIdAsync(idCuenta.ToString());
        if (cuenta is null)
        {
            return ResultadoAuth<PerfilUsuarioDto>.Sin(
                EstadoResultadoAuth.CredencialesInvalidas,
                "La cuenta no existe o la sesión ya no es válida.");
        }

        var roles = await _usuarios.GetRolesAsync(cuenta);
        return ResultadoAuth<PerfilUsuarioDto>.Exito(MapearPerfil(cuenta, roles));
    }

    // Mensaje único para cuentas deshabilitadas, con el contacto de soporte.
    private static ResultadoAuth<RespuestaLoginDto> MensajeDeshabilitada() =>
        ResultadoAuth<RespuestaLoginDto>.Sin(
            EstadoResultadoAuth.Deshabilitada,
            $"Tu cuenta está deshabilitada. Contacta a {CorreoSoporte}.");

    // Convierte la cuenta al perfil que se devuelve a la app.
    public static PerfilUsuarioDto MapearPerfil(CuentaUsuario cuenta, IEnumerable<string> roles) =>
        new(
            cuenta.Id,
            cuenta.Email ?? string.Empty,
            cuenta.Nombre,
            cuenta.Apellido,
            cuenta.PhoneNumber ?? string.Empty,
            cuenta.FechaNacimiento,
            cuenta.SexoCodigo,
            cuenta.EstadoCodigo,
            roles.ToList());
}
