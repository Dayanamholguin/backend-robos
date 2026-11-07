using System.Net;
using System.Threading.RateLimiting;
using AppRobos.Backend.Modelos;
using AppRobos.Backend.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace AppRobos.Backend.Configuracion;

// Configura el limitador de peticiones con el middleware integrado de ASP.NET Core.
// Usa las políticas de la copia en memoria, así que los cambios en la BD se aplican tras la siguiente recarga.
public class ConfigurarLimitadorTasa : IConfigureOptions<RateLimiterOptions>
{
    private readonly ServicioConfiguracionLimiteTasa _configuracion;

    public ConfigurarLimitadorTasa(ServicioConfiguracionLimiteTasa configuracion)
    {
        _configuracion = configuracion;
    }

    public void Configure(RateLimiterOptions opciones)
    {
        // Sin cola: la petición que excede el límite recibe 429 de inmediato
        opciones.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        opciones.OnRejected = async (contexto, ct) =>
        {
            var respuesta = contexto.HttpContext.Response;
            respuesta.StatusCode = StatusCodes.Status429TooManyRequests;

            // Indica cuánto esperar antes de reintentar, si el limitador lo conoce
            if (contexto.Lease.TryGetMetadata(MetadataName.RetryAfter, out var espera))
            {
                respuesta.Headers.RetryAfter = ((int)Math.Ceiling(espera.TotalSeconds)).ToString();
            }

            await respuesta.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Demasiadas solicitudes",
                Detail = "Superaste el número de solicitudes permitido. Espera un momento antes de volver a intentarlo."
            }, ct);
        };

        // Límite general por IP para toda la API. Se aplica a cada petición además de la política del endpoint.
        opciones.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(contexto =>
        {
            var instantanea = _configuracion.Actual;
            var politica = instantanea.Obtener(NombresPoliticasLimiteTasa.PorDefecto);

            // Si la política está desactivada en la BD, no se limita esta parte
            if (politica is null || !politica.Activa)
            {
                return RateLimitPartition.GetNoLimiter("sin-limite");
            }

            var clave = ClaveParticion(contexto, politica.Nombre, instantanea.Version);
            return RateLimitPartition.GetFixedWindowLimiter(clave, _ => OpcionesVentana(politica));
        });

        // Políticas por endpoint. Solo se registran las de los endpoints que existen hoy.
        opciones.AddPolicy<string>(NombresPoliticasLimiteTasa.Login, contexto => ParticionPorPolitica(contexto, NombresPoliticasLimiteTasa.Login));
        opciones.AddPolicy<string>(NombresPoliticasLimiteTasa.Registro, contexto => ParticionPorPolitica(contexto, NombresPoliticasLimiteTasa.Registro));
        opciones.AddPolicy<string>(NombresPoliticasLimiteTasa.LecturaReportes, contexto => ParticionPorPolitica(contexto, NombresPoliticasLimiteTasa.LecturaReportes));
    }

    // Crea la partición de una política por endpoint, usando la copia vigente.
    private RateLimitPartition<string> ParticionPorPolitica(HttpContext contexto, string nombre)
    {
        var instantanea = _configuracion.Actual;
        var politica = instantanea.Obtener(nombre);

        // Respaldo: si por alguna razón no existe, se usa el límite de la política por defecto
        politica ??= instantanea.Obtener(NombresPoliticasLimiteTasa.PorDefecto);
        if (politica is null)
        {
            return RateLimitPartition.GetNoLimiter("sin-limite");
        }

        var clave = ClaveParticion(contexto, nombre, instantanea.Version);
        return RateLimitPartition.GetFixedWindowLimiter(clave, _ => OpcionesVentana(politica));
    }

    // Clave de partición: nombre de la política, versión de la configuración y la IP real de conexión.
    // La IP sale de la conexión y no de encabezados que envía el cliente, para que no pueda cambiarla.
    private static string ClaveParticion(HttpContext contexto, string nombre, long version)
    {
        var ip = contexto.Connection.RemoteIpAddress ?? IPAddress.None;
        return $"{nombre}:{version}:{ip}";
    }

    // Opciones de la ventana fija a partir de la política.
    private static FixedWindowRateLimiterOptions OpcionesVentana(PoliticaLimiteTasa politica) => new()
    {
        PermitLimit = politica.LimitePeticiones,
        Window = TimeSpan.FromSeconds(politica.VentanaSegundos),
        QueueLimit = 0,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        AutoReplenishment = true
    };
}
