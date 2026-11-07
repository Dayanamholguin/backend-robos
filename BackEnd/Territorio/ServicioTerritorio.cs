using AppRobos.Backend.Datos;
using AppRobos.Backend.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AppRobos.Backend.Territorio;

// Consultas geográficas y de territorio: cobertura, municipios y barrios.
// Los valores que llegan del cliente siempre van como parámetros, nunca dentro del texto del SQL.
public class ServicioTerritorio
{
    private readonly ContextoAplicacion _contexto;

    public ServicioTerritorio(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // TRUE si la ubicación está dentro del área metropolitana (función fn_validar_cobertura).
    public Task<bool> UbicacionEnCoberturaAsync(double latitud, double longitud, CancellationToken ct) =>
        _contexto.Database.SqlQuery<bool>($"SELECT fn_validar_cobertura({latitud}, {longitud}) AS \"Value\"").SingleAsync(ct);

    // TRUE si el punto cae dentro del polígono del barrio indicado.
    public Task<bool> BarrioContieneUbicacionAsync(int barrioId, double latitud, double longitud, CancellationToken ct) =>
        _contexto.Database.SqlQuery<bool>($"""
            SELECT EXISTS (
                SELECT 1 FROM "TerritorioBarrios" b
                WHERE b."Id" = {barrioId}
                  AND ST_Contains(b."Geom", ST_SetSRID(ST_MakePoint({longitud}, {latitud}), 4326))
            ) AS "Value"
            """).SingleAsync(ct);

    // TRUE si el barrio existe y pertenece al municipio indicado.
    public Task<bool> BarrioPerteneceAMunicipioAsync(int barrioId, string municipioCodigo, CancellationToken ct) =>
        _contexto.TerritorioBarrios.AnyAsync(b => b.Id == barrioId && b.MunicipioCodigo == municipioCodigo, ct);

    // Municipio y barrio del punto del mapa. Devuelve null si el punto está fuera del área metropolitana.
    public async Task<UbicacionTerritorialRespuestaDto?> UbicarAsync(double latitud, double longitud, CancellationToken ct)
    {
        if (!await UbicacionEnCoberturaAsync(latitud, longitud, ct))
        {
            return null;
        }

        // ST_Covers incluye los bordes del polígono, así que un punto sobre el límite entre dos barrios cuenta
        var barrio = await _contexto.Database.SqlQuery<FilaBarrioUbicado>($"""
            SELECT b."Id" AS "BarrioId", b."Nombre" AS "BarrioNombre",
                   m."Codigo" AS "MunicipioCodigo", m."Nombre" AS "MunicipioNombre"
            FROM "TerritorioBarrios" b
            JOIN "TerritorioMunicipios" m ON m."Codigo" = b."MunicipioCodigo"
            WHERE ST_Covers(b."Geom", ST_SetSRID(ST_MakePoint({longitud}, {latitud}), 4326))
            LIMIT 1
            """).FirstOrDefaultAsync(ct);

        if (barrio is null)
        {
            return new UbicacionTerritorialRespuestaDto
            {
                Latitud = latitud,
                Longitud = longitud,
                Aviso = "La ubicación está en el área metropolitana, pero no tiene barrio cargado.",
            };
        }

        return new UbicacionTerritorialRespuestaDto
        {
            Latitud = latitud,
            Longitud = longitud,
            MunicipioCodigo = barrio.MunicipioCodigo,
            MunicipioNombre = barrio.MunicipioNombre,
            BarrioId = barrio.BarrioId,
            BarrioNombre = barrio.BarrioNombre,
        };
    }

    // Columnas que devuelve la consulta de barrio del punto. Va aparte porque SqlQuery exige todas las propiedades del tipo.
    private sealed class FilaBarrioUbicado
    {
        public int BarrioId { get; set; }
        public string BarrioNombre { get; set; } = string.Empty;
        public string MunicipioCodigo { get; set; } = string.Empty;
        public string MunicipioNombre { get; set; } = string.Empty;
    }

    // Municipios que el ciudadano puede elegir.
    public async Task<IReadOnlyList<OpcionCatalogoRespuestaDto>> MunicipiosAsync(CancellationToken ct)
    {
        var municipios = await _contexto.TerritorioMunicipios
            .AsNoTracking()
            .OrderBy(m => m.Nombre)
            .ToListAsync(ct);

        return municipios
            .Select((m, indice) => new OpcionCatalogoRespuestaDto { Codigo = m.Codigo, Etiqueta = m.Nombre, Orden = indice + 1 })
            .ToList();
    }

    // Barrios o sectores de un municipio. El texto de búsqueda filtra con ILIKE parametrizado.
    public async Task<IReadOnlyList<BarrioRespuestaDto>> BarriosAsync(string municipioCodigo, string? busqueda, CancellationToken ct)
    {
        var consulta = _contexto.TerritorioBarrios
            .AsNoTracking()
            .Where(b => b.MunicipioCodigo == municipioCodigo);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var patron = $"%{busqueda.Trim()}%";
            consulta = consulta.Where(b => EF.Functions.ILike(b.Nombre, patron));
        }

        return await consulta
            .OrderBy(b => b.Nombre)
            .Select(b => new BarrioRespuestaDto { Id = b.Id, Nombre = b.Nombre, MunicipioCodigo = b.MunicipioCodigo })
            .Take(200)
            .ToListAsync(ct);
    }
}
