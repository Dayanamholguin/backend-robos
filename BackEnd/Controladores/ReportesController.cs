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

    /// <summary>
    /// Crea un borrador. Puede tener solo algunos campos; los obligatorios se revisan al enviar.
    /// </summary>
    /// <remarks>
    /// El borrador vence a los 15 días desde el primer guardado. Si no se envía antes, se borra automáticamente.
    /// Los campos de catálogo se envían como código. Los campos que no aplican se descartan al guardar.
    /// </remarks>
    /// <response code="201">Borrador creado.</response>
    /// <response code="400">Algún valor no es válido o no existe en su lista.</response>
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

    /// <summary>
    /// Lista los reportes del ciudadano que tiene la sesión.
    /// </summary>
    /// <remarks>
    /// Muestra los borradores y los reportes enviados, del más reciente al más antiguo. Devuelve como máximo 100.
    /// </remarks>
    [HttpGet]
    [EnableRateLimiting(NombresPoliticasLimiteTasa.LecturaReportes)]
    [ProducesResponseType(typeof(IReadOnlyList<ReporteResumenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        return Ok(await _reportes.ListarAsync(ct));
    }

    /// <summary>
    /// Devuelve un reporte con todas sus secciones.
    /// </summary>
    /// <response code="200">Reporte con sus datos e ICR, si ya fue enviado.</response>
    /// <response code="404">El reporte no existe o pertenece a otra cuenta.</response>
    [HttpGet("{id:guid}", Name = nameof(Obtener))]
    [EnableRateLimiting(NombresPoliticasLimiteTasa.LecturaReportes)]
    [ProducesResponseType(typeof(ReporteRespuestaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(Guid id, CancellationToken ct)
    {
        return Responder(await _reportes.ObtenerAsync(id, ct));
    }

    /// <summary>
    /// Reemplaza los datos de un borrador.
    /// </summary>
    /// <remarks>
    /// Solo el autor puede editar su borrador. Los datos anteriores se reemplazan por completo.
    /// </remarks>
    /// <response code="200">Borrador actualizado.</response>
    /// <response code="400">Algún valor no es válido o no existe en su lista.</response>
    /// <response code="404">El reporte no existe o pertenece a otra cuenta.</response>
    /// <response code="409">El reporte ya fue enviado y no se puede modificar.</response>
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

    /// <summary>
    /// Borra un borrador. Un reporte enviado no se puede borrar.
    /// </summary>
    /// <response code="204">Borrador eliminado.</response>
    /// <response code="404">El reporte no existe o pertenece a otra cuenta.</response>
    /// <response code="409">El reporte ya fue enviado.</response>
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

    /// <summary>
    /// Envía el reporte: revisa los obligatorios, calcula el ICR y lo deja inmutable.
    /// </summary>
    /// <remarks>
    /// Revisa las secciones A, B, C, D, F y H. Las secciones E y G son opcionales.
    /// Calcula el ICR (0 a 100) y su clasificación. Después del envío el reporte no se puede modificar ni borrar.
    /// </remarks>
    /// <response code="200">Reporte enviado. Devuelve el ICR y su detalle por criterio.</response>
    /// <response code="400">Faltan datos obligatorios. El detalle indica cuáles.</response>
    /// <response code="404">El reporte no existe o pertenece a otra cuenta.</response>
    /// <response code="409">El reporte ya fue enviado.</response>
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
