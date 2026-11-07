using AppRobos.Backend.Catalogos;
using AppRobos.Backend.Territorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppRobos.Backend.Controladores;

// Listas desplegables del formulario y territorio (municipios y barrios). Ruta base: /api/catalogos.
[ApiController]
[Route("api/catalogos")]
[Authorize]
[Produces("application/json")]
public class CatalogosController : ControllerBase
{
    private readonly ServicioOpcionesFormulario _opciones;
    private readonly ServicioTerritorio _territorio;

    public CatalogosController(ServicioOpcionesFormulario opciones, ServicioTerritorio territorio)
    {
        _opciones = opciones;
        _territorio = territorio;
    }

    // Municipios que el ciudadano puede elegir.
    [HttpGet("municipios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Municipios(CancellationToken ct) => Ok(await _territorio.MunicipiosAsync(ct));

    // Barrios o sectores del municipio. El parámetro busqueda filtra por texto.
    [HttpGet("municipios/{codigo}/barrios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Barrios(string codigo, [FromQuery] string? busqueda, CancellationToken ct) =>
        Ok(await _territorio.BarriosAsync(codigo.Trim().ToUpperInvariant(), busqueda, ct));

    // Opciones habilitadas de una lista (por ejemplo, CondicionClima). 404 si la lista no existe.
    [HttpGet("{catalogo}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Opciones(string catalogo, CancellationToken ct)
    {
        var opciones = await _opciones.ListarAsync(catalogo, ct);
        return opciones.Count == 0
            ? NotFound()
            : Ok(opciones);
    }
}
