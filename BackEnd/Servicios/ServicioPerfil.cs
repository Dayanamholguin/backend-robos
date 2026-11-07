using AppRobos.Backend.Dtos;
using AppRobos.Backend.Modelos;
using Microsoft.AspNetCore.Identity;

namespace AppRobos.Backend.Servicios;

// Edición del perfil del propio ciudadano (RF-003) y cambio de contraseña.
// Solo se pueden editar datos personales y la contraseña. No se editan el correo, el rol ni el estado.
// No hay foto de perfil.
public class ServicioPerfil
{
    private readonly UserManager<CuentaUsuario> _usuarios;
    private readonly ServicioCatalogos _catalogos;
    private readonly ServicioBloqueoCuenta _bloqueo;
    private readonly TimeProvider _reloj;

    public ServicioPerfil(
        UserManager<CuentaUsuario> usuarios,
        ServicioCatalogos catalogos,
        ServicioBloqueoCuenta bloqueo,
        TimeProvider reloj)
    {
        _usuarios = usuarios;
        _catalogos = catalogos;
        _bloqueo = bloqueo;
        _reloj = reloj;
    }

    // Reemplaza los datos personales de la cuenta indicada. El correo, el rol y el estado no se leen del DTO,
    // así que no pueden cambiar aunque la app los envíe.
    public async Task<ResultadoAuth<PerfilUsuarioDto>> ActualizarAsync(Guid idCuenta, ActualizarPerfilDto datos)
    {
        var cuenta = await _usuarios.FindByIdAsync(idCuenta.ToString());
        if (cuenta is null)
        {
            return SesionNoValida();
        }

        // El sexo/género debe estar en la lista habilitada de la base de datos
        if (!await _catalogos.CodigoHabilitadoAsync(CatalogosAplicacion.SexoGenero, datos.SexoCodigo))
        {
            return ResultadoAuth<PerfilUsuarioDto>.Invalido(["El sexo o género seleccionado no es válido."]);
        }

        cuenta.Nombre = datos.Nombre.Trim();
        cuenta.Apellido = datos.Apellido.Trim();
        cuenta.PhoneNumber = datos.Telefono;
        cuenta.FechaNacimiento = datos.FechaNacimiento;
        cuenta.SexoCodigo = datos.SexoCodigo;
        cuenta.FechaActualizacion = _reloj.GetUtcNow();

        var resultado = await _usuarios.UpdateAsync(cuenta);
        if (!resultado.Succeeded)
        {
            return ResultadoAuth<PerfilUsuarioDto>.Invalido(TraductorErroresIdentity.Traducir(resultado.Errors));
        }

        var roles = await _usuarios.GetRolesAsync(cuenta);
        return ResultadoAuth<PerfilUsuarioDto>.Exito(ServicioAutenticacion.MapearPerfil(cuenta, roles));
    }

    // Cambia la contraseña verificando la actual. Un intento con contraseña actual incorrecta cuenta
    // para el bloqueo escalonado, igual que en el login, para que no sirva para adivinar contraseñas.
    public async Task<ResultadoAuth<PerfilUsuarioDto>> CambiarContrasenaAsync(Guid idCuenta, CambiarContrasenaDto datos)
    {
        var ahora = _reloj.GetUtcNow();
        var cuenta = await _usuarios.FindByIdAsync(idCuenta.ToString());
        if (cuenta is null)
        {
            return SesionNoValida();
        }

        // Una cuenta bloqueada no puede cambiar la contraseña hasta que termine el bloqueo
        _bloqueo.LiberarBloqueoVencido(cuenta, ahora);
        if (cuenta.CuentaDeshabilitada)
        {
            return ResultadoAuth<PerfilUsuarioDto>.Sin(EstadoResultadoAuth.Deshabilitada, MensajeDeshabilitada());
        }
        if (_bloqueo.EstaBloqueadaTemporalmente(cuenta, ahora))
        {
            return ResultadoAuth<PerfilUsuarioDto>.Bloqueo(cuenta.LockoutEnd!.Value);
        }

        if (datos.ContrasenaActual == datos.ContrasenaNueva)
        {
            return ResultadoAuth<PerfilUsuarioDto>.Invalido(["La contraseña nueva debe ser distinta de la actual."]);
        }

        // Identity verifica la contraseña actual, aplica la política y guarda solo el hash de la nueva
        var resultado = await _usuarios.ChangePasswordAsync(cuenta, datos.ContrasenaActual, datos.ContrasenaNueva);
        if (resultado.Succeeded)
        {
            // Renovar el sello invalida todos los tokens emitidos antes: se cierran todas las sesiones abiertas
            await _usuarios.UpdateSecurityStampAsync(cuenta);
            cuenta.FechaActualizacion = ahora;
            await _usuarios.UpdateAsync(cuenta);

            var roles = await _usuarios.GetRolesAsync(cuenta);
            return ResultadoAuth<PerfilUsuarioDto>.Exito(ServicioAutenticacion.MapearPerfil(cuenta, roles));
        }

        // Contraseña actual incorrecta: se cuenta como intento fallido
        if (resultado.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordMismatch)))
        {
            var fallo = _bloqueo.RegistrarFallo(cuenta, ahora);
            await _usuarios.UpdateAsync(cuenta);

            return fallo switch
            {
                ResultadoFalloLogin.BloqueoTemporal => ResultadoAuth<PerfilUsuarioDto>.Bloqueo(cuenta.LockoutEnd!.Value),
                ResultadoFalloLogin.BloqueoPermanente => ResultadoAuth<PerfilUsuarioDto>.Sin(EstadoResultadoAuth.Deshabilitada, MensajeDeshabilitada()),
                _ => ResultadoAuth<PerfilUsuarioDto>.Invalido(["La contraseña actual no es correcta."])
            };
        }

        // Otros errores: la nueva contraseña no cumple la política de Identity
        return ResultadoAuth<PerfilUsuarioDto>.Invalido(TraductorErroresIdentity.Traducir(resultado.Errors));
    }

    // Respuesta cuando el token apunta a una cuenta que ya no existe.
    private static ResultadoAuth<PerfilUsuarioDto> SesionNoValida() =>
        ResultadoAuth<PerfilUsuarioDto>.Sin(
            EstadoResultadoAuth.CredencialesInvalidas,
            "La cuenta no existe o la sesión ya no es válida.");

    // Mensaje de cuenta deshabilitada con el contacto de soporte.
    private static string MensajeDeshabilitada() =>
        $"Tu cuenta está deshabilitada. Contacta a {ServicioAutenticacion.CorreoSoporte}.";
}
