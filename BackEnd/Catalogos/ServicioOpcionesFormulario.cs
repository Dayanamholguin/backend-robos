using AppRobos.Backend.Datos;
using AppRobos.Backend.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AppRobos.Backend.Catalogos;

// Lee las listas desplegables de la base de datos.
// Carga todas las opciones en una sola consulta para validar un reporte completo.
public class ServicioOpcionesFormulario
{
    private readonly ContextoAplicacion _contexto;

    public ServicioOpcionesFormulario(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // Opciones habilitadas de un catálogo, ordenadas para mostrarlas en la app.
    // Devuelve una lista vacía si el catálogo no existe o no tiene opciones habilitadas.
    public async Task<IReadOnlyList<OpcionCatalogoRespuestaDto>> ListarAsync(string catalogo, CancellationToken ct)
    {
        return await _contexto.CatalogosOpciones
            .AsNoTracking()
            .Where(o => o.Catalogo == catalogo && o.Habilitado)
            .OrderBy(o => o.Orden)
            .Select(o => new OpcionCatalogoRespuestaDto { Codigo = o.Codigo, Etiqueta = o.Etiqueta, Orden = o.Orden })
            .ToListAsync(ct);
    }

    // Carga todas las opciones para validar y calcular la completitud sin consultar fila por fila.
    public async Task<OpcionesCargadas> CargarAsync(CancellationToken ct)
    {
        var filas = await _contexto.CatalogosOpciones
            .AsNoTracking()
            .Select(o => new { o.Catalogo, o.Codigo, o.Habilitado, o.CuentaComoDato })
            .ToListAsync(ct);

        return new OpcionesCargadas(filas.Select(f => (f.Catalogo, f.Codigo, f.Habilitado, f.CuentaComoDato)));
    }
}

// Opciones de catálogo ya cargadas en memoria.
public sealed class OpcionesCargadas
{
    private readonly HashSet<(string, string)> _habilitadas = new();
    private readonly HashSet<(string, string)> _sinDato = new();

    public OpcionesCargadas(IEnumerable<(string Catalogo, string Codigo, bool Habilitado, bool CuentaComoDato)> filas)
    {
        foreach (var (catalogo, codigo, habilitado, cuentaComoDato) in filas)
        {
            if (habilitado)
            {
                _habilitadas.Add((catalogo, codigo));
            }
            if (!cuentaComoDato)
            {
                _sinDato.Add((catalogo, codigo));
            }
        }
    }

    // TRUE si el código existe en el catálogo y está habilitado.
    public bool Existe(string catalogo, string? codigo) =>
        codigo is not null && _habilitadas.Contains((catalogo, codigo));

    // TRUE si el código existe y además cuenta como dato para la completitud.
    public bool EsDato(string catalogo, string? codigo) =>
        Existe(catalogo, codigo) && !_sinDato.Contains((catalogo, codigo!));
}
