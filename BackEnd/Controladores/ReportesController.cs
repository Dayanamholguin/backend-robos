using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Dtos;
using AppRobos.Backend.Reportes;
using AppRobos.Backend.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AppRobos.Backend.Controladores;

// Reportes de hurtos del ciudadano. Ruta base: /api/reportes.
// Cada endpoint trabaja solo con los reportes de la cuenta que tiene la sesión.
[ApiController]
[Route("api/reportes")]
[Authorize]
[Produces("application/json")]
public class ReportesController : ControllerBase
{
    private readonly ServicioReportes _reportes;

    public ReportesController(ServicioReportes reportes)
    {
        _reportes = reportes;
    }

    // Crea un borrador. Puede tener solo algunos campos; los obligatorios se revisan al enviar.
    [HttpPost]
    [ProducesResponseType(typeof(ReporteRespuestaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearBorrador([FromBody] ReporteEntradaDto datos, CancellationToken ct)
    {
        if (ObtenerUsuario() is not { } usuarioId)
        {
            return Problema(StatusCodes.Status401Unauthorized, "No autorizado", "El token no identifica una cuenta válida.");
        }

        var resultado = await _reportes.CrearBorradorAsync(usuarioId, datos, ct);
        return resultado.Tipo == TipoResultadoReporte.Exito
            ? CreatedAtAction(nameof(Obtener), new { id = resultado.Datos!.Id }, resultado.Datos)
            : Responder(resultado);
    }

    // Lista los reportes del ciudadano.
    [HttpGet]
    [EnableRateLimiting(NombresPoliticasLimiteTasa.LecturaReportes)]
    [ProducesResponseType(typeof(IReadOnlyList<ReporteResumenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        return Ok(await _reportes.ListarAsync(ct));
    }

    // Devuelve un reporte con todas sus secciones.
    [HttpGet("{id:guid}", Name = nameof(Obtener))]
    [EnableRateLimiting(NombresPoliticasLimiteTasa.LecturaReportes)]
    [ProducesResponseType(typeof(ReporteRespuestaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(Guid id, CancellationToken ct)
    {
        return Responder(await _reportes.ObtenerAsync(id, ct));
    }

    // Reemplaza los datos de un borrador.
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ReporteRespuestaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActualizarBorrador(Guid id, [FromBody] ReporteEntradaDto datos, CancellationToken ct)
    {
        if (ObtenerUsuario() is not { } usuarioId)
        {
            return Problema(StatusCodes.Status401Unauthorized, "No autorizado", "El token no identifica una cuenta válida.");
        }

        return Responder(await _reportes.ActualizarBorradorAsync(usuarioId, id, datos, ct));
    }

    // Borra un borrador. Un reporte enviado no se puede borrar.
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EliminarBorrador(Guid id, CancellationToken ct)
    {
        if (ObtenerUsuario() is not { } usuarioId)
        {
            return Unauthorized();
        }

        var resultado = await _reportes.EliminarBorradorAsync(usuarioId, id, ct);
        return resultado.Tipo == TipoResultadoReporte.Exito ? NoContent() : Responder(resultado);
    }

    // Envía el reporte: revisa los obligatorios, calcula el ICR y lo deja inmutable.
    [HttpPost("{id:guid}/enviar")]
    [ProducesResponseType(typeof(ReporteRespuestaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enviar(Guid id, CancellationToken ct)
    {
        if (ObtenerUsuario() is not { } usuarioId)
        {
            return Unauthorized();
        }

        return Responder(await _reportes.EnviarAsync(usuarioId, id, ct));
    }

    private Guid? ObtenerUsuario() => User.ObtenerIdCuenta();

    // Traduce el resultado del servicio a una respuesta HTTP con formato ProblemDetails.
    private IActionResult Responder(ResultadoReporte resultado) => resultado.Tipo switch
    {
        TipoResultadoReporte.Exito when resultado.Datos is not null => Ok(resultado.Datos),
        TipoResultadoReporte.Exito => NoContent(),
        TipoResultadoReporte.Invalido => Problema(StatusCodes.Status400BadRequest, "Datos no válidos", string.Join(" ", resultado.Mensajes)),
        TipoResultadoReporte.Conflicto => Problema(StatusCodes.Status409Conflict, "Conflicto", string.Join(" ", resultado.Mensajes)),
        _ => Problema(StatusCodes.Status404NotFound, "No encontrado", "El reporte no existe o no está disponible para esta cuenta."),
    };

    private IActionResult Problema(int estado, string titulo, string detalle) =>
        new ObjectResult(new Microsoft.AspNetCore.Mvc.ProblemDetails { Status = estado, Title = titulo, Detail = detalle }) { StatusCode = estado };
}
