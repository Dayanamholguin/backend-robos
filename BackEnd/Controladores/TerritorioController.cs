using AppRobos.Backend.Dtos;
using AppRobos.Backend.Territorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppRobos.Backend.Controladores;

// Ubicación territorial de un punto del mapa. Ruta base: /api/territorio.
[ApiController]
[Route("api/territorio")]
[Authorize]
[Produces("application/json")]
public class TerritorioController : ControllerBase
{
    private readonly ServicioTerritorio _territorio;

    public TerritorioController(ServicioTerritorio territorio)
    {
        _territorio = territorio;
    }

    /// <summary>
    /// Devuelve municipio y barrio del punto seleccionado en el mapa.
    /// </summary>
    /// <remarks>
    /// Fuera del área metropolitana responde 422 y la app no deja continuar.
    /// Si el punto está en el área pero no tiene barrio cargado, responde 200 con los campos de municipio y barrio vacíos y un aviso.
    /// </remarks>
    /// <response code="200">Municipio y barrio del punto.</response>
    /// <response code="400">Latitud o longitud fuera de rango.</response>
    /// <response code="422">El punto está fuera del área metropolitana.</response>
    [HttpGet("ubicacion")]
    [ProducesResponseType(typeof(UbicacionTerritorialRespuestaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Ubicacion([FromQuery] UbicacionConsultaDto consulta, CancellationToken ct)
    {
        var resultado = await _territorio.UbicarAsync(consulta.Latitud, consulta.Longitud, ct);
        if (resultado is null)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Ubicación no habilitada",
                detail: "La ubicación no está habilitada para esta zona.");
        }

        return Ok(resultado);
    }
}
