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

    // Devuelve municipio y barrio del punto seleccionado en el mapa.
    // Fuera del área metropolitana responde 422: la app no deja continuar.
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
