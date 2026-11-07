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

    /// <summary>
    /// Municipios del área metropolitana que el ciudadano puede elegir.
    /// </summary>
    [HttpGet("municipios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Municipios(CancellationToken ct) => Ok(await _territorio.MunicipiosAsync(ct));

    /// <summary>
    /// Barrios o sectores de un municipio.
    /// </summary>
    /// <remarks>
    /// El parámetro busqueda filtra por texto sin distinguir mayúsculas. Devuelve como máximo 200.
    /// </remarks>
    /// <param name="codigo">Código del municipio, por ejemplo MEDELLIN.</param>
    [HttpGet("municipios/{codigo}/barrios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Barrios(string codigo, [FromQuery] string? busqueda, CancellationToken ct) =>
        Ok(await _territorio.BarriosAsync(codigo.Trim().ToUpperInvariant(), busqueda, ct));

    /// <summary>
    /// Opciones habilitadas de una lista del formulario.
    /// </summary>
    /// <remarks>
    /// Ejemplo de listas: CondicionClima, IntensidadLluvia. Responde 404 si la lista no existe.
    /// </remarks>
    /// <param name="catalogo">Nombre de la lista.</param>
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
