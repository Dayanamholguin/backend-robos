using System.IdentityModel.Tokens.Jwt;
using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Dtos;
using AppRobos.Backend.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AppRobos.Backend.Controladores;

// Endpoints de autenticación del ciudadano: registro, inicio de sesión y perfil.
// Ruta base: /api/autenticacion.
[ApiController]
[Route("api/autenticacion")]
[Produces("application/json")]
public class AutenticacionController : ControllerBase
{
    private readonly ServicioAutenticacion _autenticacion;
    private readonly ServicioPerfil _perfil;

    public AutenticacionController(ServicioAutenticacion autenticacion, ServicioPerfil perfil)
    {
        _autenticacion = autenticacion;
        _perfil = perfil;
    }

    /// <summary>
    /// Registra un ciudadano nuevo con el rol Ciudadano (RF-001).
    /// </summary>
    /// <remarks>
    /// El correo no puede estar registrado. El sexo/género debe ser uno de los valores del catálogo.
    /// La cuenta queda activa y no se puede elegir el rol: siempre es Ciudadano.
    /// </remarks>
    /// <response code="201">Cuenta creada. Devuelve el perfil.</response>
    /// <response code="400">Algún dato no cumple las reglas de validación.</response>
    /// <response code="409">El correo ya está registrado.</response>
    [AllowAnonymous]
    [EnableRateLimiting(NombresPoliticasLimiteTasa.Registro)]
    [HttpPost("registro")]
    [ProducesResponseType(typeof(PerfilUsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDto datos)
    {
        var resultado = await _autenticacion.RegistrarAsync(datos);

        // Si el registro fue exitoso se responde 201 con el perfil creado
        return resultado.Estado == EstadoResultadoAuth.Exitoso
            ? StatusCode(StatusCodes.Status201Created, resultado.Datos)
            : Responder(resultado);
    }

    /// <summary>
    /// Inicia sesión y devuelve un token JWT (RF-002).
    /// </summary>
    /// <remarks>
    /// Tras 3 intentos fallidos la cuenta se bloquea temporalmente: 3 horas, luego 1 día y luego 1 semana.
    /// Desde el cuarto bloqueo la cuenta se deshabilita y solo se reactiva por contacto con app@pruebas.com.
    /// Un inicio de sesión exitoso reinicia el escalonamiento.
    /// </remarks>
    /// <response code="200">Sesión iniciada. Devuelve el token y sus datos de vencimiento.</response>
    /// <response code="401">Correo o contraseña incorrectos.</response>
    /// <response code="403">La cuenta está deshabilitada o inactiva.</response>
    /// <response code="423">La cuenta está bloqueada. El campo bloqueadaHasta indica hasta cuándo.</response>
    [AllowAnonymous]
    [EnableRateLimiting(NombresPoliticasLimiteTasa.Login)]
    [HttpPost("login")]
    [ProducesResponseType(typeof(RespuestaLoginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status423Locked)]
    public async Task<IActionResult> IniciarSesion([FromBody] LoginUsuarioDto datos)
    {
        var resultado = await _autenticacion.IniciarSesionAsync(datos);

        return resultado.Estado == EstadoResultadoAuth.Exitoso
            ? Ok(resultado.Datos)
            : Responder(resultado);
    }

    /// <summary>
    /// Devuelve el perfil del ciudadano que tiene la sesión activa (RF-003).
    /// </summary>
    /// <response code="200">Perfil del ciudadano.</response>
    /// <response code="401">Falta el token o no es válido.</response>
    [Authorize]
    [HttpGet("perfil")]
    [ProducesResponseType(typeof(PerfilUsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObtenerPerfil()
    {
        if (!TryObtenerIdCuenta(out var idCuenta))
        {
            return Problema(StatusCodes.Status401Unauthorized, "No autorizado", "El token no identifica una cuenta válida.");
        }

        var resultado = await _autenticacion.ObtenerPerfilAsync(idCuenta);
        return resultado.Estado == EstadoResultadoAuth.Exitoso
            ? Ok(resultado.Datos)
            : Responder(resultado);
    }

    /// <summary>
    /// Edita los datos personales del ciudadano que tiene la sesión (RF-003).
    /// </summary>
    /// <remarks>
    /// Solo se editan nombre, apellido, teléfono, fecha de nacimiento y sexo/género.
    /// El correo, el rol y el estado no se pueden cambiar; cualquier valor de esos campos se ignora.
    /// </remarks>
    /// <response code="200">Perfil actualizado.</response>
    /// <response code="400">Algún dato no cumple las reglas de validación.</response>
    /// <response code="401">Falta el token o no es válido.</response>
    [Authorize]
    [HttpPut("perfil")]
    [ProducesResponseType(typeof(PerfilUsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDto datos)
    {
        if (!TryObtenerIdCuenta(out var idCuenta))
        {
            return Problema(StatusCodes.Status401Unauthorized, "No autorizado", "El token no identifica una cuenta válida.");
        }

        var resultado = await _perfil.ActualizarAsync(idCuenta, datos);
        return resultado.Estado == EstadoResultadoAuth.Exitoso
            ? Ok(resultado.Datos)
            : Responder(resultado);
    }

    /// <summary>
    /// Cambia la contraseña del ciudadano con sesión activa, verificando la contraseña actual.
    /// </summary>
    /// <remarks>
    /// Al cambiar la contraseña se cierran todas las sesiones abiertas, incluida esta.
    /// Un intento con la contraseña actual incorrecta cuenta para el bloqueo escalonado.
    /// </remarks>
    /// <response code="200">Contraseña cambiada. Las sesiones anteriores quedan cerradas.</response>
    /// <response code="400">La contraseña nueva no cumple la política.</response>
    /// <response code="401">Falta el token o no es válido.</response>
    /// <response code="403">La contraseña actual es incorrecta.</response>
    /// <response code="423">La cuenta está bloqueada por intentos fallidos.</response>
    [Authorize]
    [HttpPut("perfil/contrasena")]
    [ProducesResponseType(typeof(PerfilUsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status423Locked)]
    public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaDto datos)
    {
        if (!TryObtenerIdCuenta(out var idCuenta))
        {
            return Problema(StatusCodes.Status401Unauthorized, "No autorizado", "El token no identifica una cuenta válida.");
        }

        var resultado = await _perfil.CambiarContrasenaAsync(idCuenta, datos);
        return resultado.Estado == EstadoResultadoAuth.Exitoso
            ? Ok(resultado.Datos)
            : Responder(resultado);
    }

    // Lee el identificador de la cuenta del claim "sub" del token. Devuelve false si no es válido.
    private bool TryObtenerIdCuenta(out Guid idCuenta)
    {
        var idTexto = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(idTexto, out idCuenta);
    }

    // Traduce un resultado de error de autenticación a una respuesta HTTP con formato ProblemDetails.
    private IActionResult Responder<T>(ResultadoAuth<T> resultado)
    {
        var detalle = resultado.Mensajes is null ? string.Empty : string.Join(" ", resultado.Mensajes);

        return resultado.Estado switch
        {
            EstadoResultadoAuth.Invalido => Problema(StatusCodes.Status400BadRequest, "Datos no válidos", detalle),
            EstadoResultadoAuth.Conflicto => Problema(StatusCodes.Status409Conflict, "Conflicto", detalle),
            EstadoResultadoAuth.CredencialesInvalidas => Problema(StatusCodes.Status401Unauthorized, "Credenciales inválidas", detalle),
            EstadoResultadoAuth.Bloqueada => Problema(
                StatusCodes.Status423Locked,
                "Cuenta bloqueada temporalmente",
                "Demasiados intentos fallidos. Intenta de nuevo después de la fecha indicada.",
                resultado.BloqueadoHasta),
            EstadoResultadoAuth.Deshabilitada => Problema(StatusCodes.Status403Forbidden, "Cuenta deshabilitada", detalle),
            EstadoResultadoAuth.Inactiva => Problema(StatusCodes.Status403Forbidden, "Cuenta inactiva", detalle),
            _ => Problema(StatusCodes.Status500InternalServerError, "Error interno", "No se pudo completar la operación.")
        };
    }

    // Crea una respuesta ProblemDetails con el código indicado y, si aplica, la fecha de fin del bloqueo.
    private IActionResult Problema(int estado, string titulo, string detalle, DateTimeOffset? bloqueadaHasta = null)
    {
        var problema = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = estado,
            Title = titulo,
            Detail = detalle
        };

        // Se informa la fecha de fin del bloqueo en UTC para que la app la muestre en la hora local
        if (bloqueadaHasta.HasValue)
        {
            problema.Extensions["bloqueadaHasta"] = bloqueadaHasta.Value.UtcDateTime;
        }

        return new ObjectResult(problema) { StatusCode = estado };
    }
}
