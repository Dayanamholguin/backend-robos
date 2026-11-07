using System.Net.Sockets;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AppRobos.Backend.Errores;

// Convierte los fallos de conexión a PostgreSQL en 503 Service Unavailable con un mensaje en español.
// Sin este manejador, una caída de la base responde 500 genérico.
// Los demás errores no los maneja: siguen su curso normal.
public class ManejadorBaseDatosNoDisponible : IExceptionHandler
{
    // Segundos que se sugiere esperar antes de reintentar.
    private const int SegundosReintento = 30;

    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excepcion, CancellationToken ct)
    {
        // Si el error no es de conexión, se deja para el manejo normal
        if (!EsFalloDeConexion(excepcion))
        {
            return false;
        }

        contexto.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        contexto.Response.Headers.RetryAfter = SegundosReintento.ToString();

        await contexto.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Servicio no disponible",
            Detail = "La base de datos no responde en este momento. Intenta de nuevo en unos minutos."
        }, ct);

        return true;
    }

    // Revisa la cadena de excepciones internas. Un error de Npgsql o de socket indica que la base no responde.
    // Los errores de SQL (PostgresException) no se consideran fallo de conexión.
    public static bool EsFalloDeConexion(Exception excepcion)
    {
        for (Exception? actual = excepcion; actual is not null; actual = actual.InnerException)
        {
            if (actual is SocketException or TimeoutException ||
                (actual is NpgsqlException && actual is not PostgresException))
            {
                return true;
            }
        }
        return false;
    }
}

