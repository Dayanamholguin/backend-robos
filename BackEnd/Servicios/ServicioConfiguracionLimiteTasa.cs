using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Datos;
using AppRobos.Backend.Modelos;
using Microsoft.EntityFrameworkCore;

namespace AppRobos.Backend.Servicios;

// Copia en memoria de las políticas de rate limiting.
// Se recarga desde la BD cada 60 segundos (TrabajoRecargaLimiteTasa). Las peticiones usan siempre esta copia,
// así que no se consulta la base de datos en cada petición.
public class ServicioConfiguracionLimiteTasa
{
    // Intervalo de recarga desde la base de datos.
    public static readonly TimeSpan IntervaloRecarga = TimeSpan.FromSeconds(60);

    // Valores de respaldo si la BD nunca respondió. Son los mismos que la migración inserta.
    private static readonly IReadOnlyDictionary<string, PoliticaLimiteTasa> Respaldo = new Dictionary<string, PoliticaLimiteTasa>
    {
        [NombresPoliticasLimiteTasa.Login] = Crear(NombresPoliticasLimiteTasa.Login, 5, 60),
        [NombresPoliticasLimiteTasa.Registro] = Crear(NombresPoliticasLimiteTasa.Registro, 3, 60),
        [NombresPoliticasLimiteTasa.LecturaReportes] = Crear(NombresPoliticasLimiteTasa.LecturaReportes, 60, 60),
        [NombresPoliticasLimiteTasa.PorDefecto] = Crear(NombresPoliticasLimiteTasa.PorDefecto, 100, 60),
    };

    private readonly IServiceScopeFactory _fabricaAmbitos;
    private readonly ILogger<ServicioConfiguracionLimiteTasa> _registro;
    private readonly object _cerrojo = new();
    private InstantaneaPoliticas _actual;

    public ServicioConfiguracionLimiteTasa(IServiceScopeFactory fabricaAmbitos, ILogger<ServicioConfiguracionLimiteTasa> registro)
    {
        _fabricaAmbitos = fabricaAmbitos;
        _registro = registro;
        _actual = new InstantaneaPoliticas(1, Respaldo);
    }

    // Copia vigente de las políticas. Se lee sin bloqueo porque la referencia se reemplaza completa.
    public InstantaneaPoliticas Actual => Volatile.Read(ref _actual);

    // Lee las políticas de la base de datos y las aplica si son válidas.
    // Si la BD no responde, se conserva la última copia válida.
    public async Task RecargarAsync(CancellationToken ct)
    {
        try
        {
            using var ambito = _fabricaAmbitos.CreateScope();
            var contexto = ambito.ServiceProvider.GetRequiredService<ContextoAplicacion>();

            // Solo lectura: no se hace seguimiento de cambios de estas entidades
            var politicas = await contexto.PoliticasLimiteTasa.AsNoTracking().ToListAsync(ct);
            AplicarPoliticas(politicas);
        }
        catch (Exception excepcion) when (excepcion is not OperationCanceledException)
        {
            _registro.LogWarning(excepcion,
                "No se pudo recargar la configuración de rate limiting. Se conserva la última configuración válida.");
        }
    }

    // Aplica una lista de políticas a la copia en memoria.
    // Una política inválida se ignora y se conserva la anterior. Si cambió algún valor, se crea una nueva versión.
    // Devuelve TRUE si hubo cambios.
    public bool AplicarPoliticas(IEnumerable<PoliticaLimiteTasa> origen)
    {
        var lista = origen.ToList();

        lock (_cerrojo)
        {
            var anterior = _actual;
            var nuevas = new Dictionary<string, PoliticaLimiteTasa>();

            foreach (var nombre in NombresPoliticasLimiteTasa.Todos)
            {
                var candidata = lista.FirstOrDefault(p => p.Nombre == nombre);
                var error = candidata is null ? "no existe en la base de datos" : ValidarPolitica(candidata);

                if (candidata is not null && error is null)
                {
                    nuevas[nombre] = candidata;
                    continue;
                }

                if (candidata is not null)
                {
                    _registro.LogWarning("La política {Nombre} no es válida ({Error}). Se usa la última válida.", nombre, error);
                }

                // Sin política válida: se conserva la anterior o, si nunca hubo, el valor de respaldo
                nuevas[nombre] = anterior.Politicas.TryGetValue(nombre, out var previa) ? previa : Respaldo[nombre];
            }

            if (!HuboCambios(anterior.Politicas, nuevas))
            {
                return false;
            }

            Volatile.Write(ref _actual, new InstantaneaPoliticas(anterior.Version + 1, nuevas));
            _registro.LogInformation("Configuración de rate limiting actualizada (versión {Version}).", anterior.Version + 1);
            return true;
        }
    }

    // Revisa que una política tenga valores permitidos.
    // Devuelve null si es válida, o el motivo del rechazo.
    public static string? ValidarPolitica(PoliticaLimiteTasa politica)
    {
        if (!NombresPoliticasLimiteTasa.Todos.Contains(politica.Nombre))
        {
            return "nombre desconocido";
        }
        if (politica.LimitePeticiones is < 1 or > 10000)
        {
            return "LimitePeticiones debe estar entre 1 y 10000";
        }
        if (politica.VentanaSegundos is < 1 or > 3600)
        {
            return "VentanaSegundos debe estar entre 1 y 3600";
        }
        if (politica.LimiteCola != 0)
        {
            return "LimiteCola debe ser 0";
        }
        if (politica.EstrategiaParticion != "IP")
        {
            return "EstrategiaParticion debe ser IP";
        }
        // Login y registro siempre deben estar protegidos
        if (politica.Nombre is NombresPoliticasLimiteTasa.Login or NombresPoliticasLimiteTasa.Registro && !politica.Activa)
        {
            return "login y registro no se pueden desactivar";
        }
        return null;
    }

    // Compara los valores que importan (límite, ventana y activa) entre dos conjuntos.
    private static bool HuboCambios(IReadOnlyDictionary<string, PoliticaLimiteTasa> anteriores, IReadOnlyDictionary<string, PoliticaLimiteTasa> nuevas)
    {
        foreach (var (nombre, nueva) in nuevas)
        {
            if (!anteriores.TryGetValue(nombre, out var anterior) ||
                anterior.LimitePeticiones != nueva.LimitePeticiones ||
                anterior.VentanaSegundos != nueva.VentanaSegundos ||
                anterior.Activa != nueva.Activa)
            {
                return true;
            }
        }
        return false;
    }

    private static PoliticaLimiteTasa Crear(string nombre, int limite, int ventana) => new()
    {
        Nombre = nombre,
        LimitePeticiones = limite,
        VentanaSegundos = ventana,
        EstrategiaParticion = "IP",
        LimiteCola = 0,
        Activa = true
    };
}

// Versión inmutable de la configuración. Cada cambio crea una versión nueva, y la versión forma parte
// de la clave de partición para que los contadores antiguos no afecten a los límites nuevos.
public sealed record InstantaneaPoliticas(long Version, IReadOnlyDictionary<string, PoliticaLimiteTasa> Politicas)
{
    // Devuelve la política por nombre o null si no existe.
    public PoliticaLimiteTasa? Obtener(string nombre) => Politicas.GetValueOrDefault(nombre);
}
