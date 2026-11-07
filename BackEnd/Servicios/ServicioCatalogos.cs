using AppRobos.Backend.Datos;
using Microsoft.EntityFrameworkCore;

namespace AppRobos.Backend.Servicios;

// Valida que un código de una lista desplegable exista y esté habilitado en la tabla CatalogoOpciones.
// Cualquier valor que no esté en la lista se rechaza.
public class ServicioCatalogos
{
    private readonly ContextoAplicacion _contexto;

    public ServicioCatalogos(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // Devuelve TRUE si el código existe en el catálogo indicado y está habilitado.
    // La consulta es parametrizada: el código del cliente nunca forma parte del SQL.
    // Nombre de la lista, por ejemplo SexoGenero.
    // Código que envió la app.
    public Task<bool> CodigoHabilitadoAsync(string catalogo, string codigo)
    {
        return _contexto.CatalogosOpciones.AnyAsync(opcion =>
            opcion.Catalogo == catalogo &&
            opcion.Codigo == codigo &&
            opcion.Habilitado);
    }
}
