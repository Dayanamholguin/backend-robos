using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AppRobos.Backend.Seguridad;

// Al abrir cada conexión a PostgreSQL, comunica el usuario actual con variables de sesión.
// Las políticas RLS leen esas variables para decidir qué filas puede ver o cambiar la petición.
// Los valores se pasan como parámetros, nunca concatenados en el SQL.
public class InterceptorSesionBaseDatos : DbConnectionInterceptor
{
    private readonly ContextoSesionBaseDatos _sesion;

    public InterceptorSesionBaseDatos(ContextoSesionBaseDatos sesion)
    {
        _sesion = sesion;
    }

    // Aplica la sesión al abrir la conexión (versión síncrona).
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        using var comando = CrearComando(connection);
        comando.ExecuteNonQuery();
    }

    // Aplica la sesión al abrir la conexión (versión asíncrona, la que usa la API).
    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await using var comando = CrearComando(connection);
        await comando.ExecuteNonQueryAsync(cancellationToken);
    }

    // Crea el comando que fija las variables de sesión de PostgreSQL.
    // El tercer argumento FALSE hace que el valor dure toda la conexión, no solo la transacción.
    private DbCommand CrearComando(DbConnection connection)
    {
        var comando = connection.CreateCommand();
        comando.CommandText = "SELECT set_config('app.usuario_id', @usuario, false), set_config('app.modo_sistema', @modo, false), set_config('app.es_superadmin', @superadmin, false)";

        var parametroUsuario = comando.CreateParameter();
        parametroUsuario.ParameterName = "@usuario";
        parametroUsuario.Value = _sesion.UsuarioId?.ToString() ?? string.Empty;
        comando.Parameters.Add(parametroUsuario);

        var parametroModo = comando.CreateParameter();
        parametroModo.ParameterName = "@modo";
        parametroModo.Value = _sesion.ModoSistema ? "on" : "off";
        comando.Parameters.Add(parametroModo);

        var parametroSuperAdmin = comando.CreateParameter();
        parametroSuperAdmin.ParameterName = "@superadmin";
        parametroSuperAdmin.Value = _sesion.EsSuperAdmin ? "on" : "off";
        comando.Parameters.Add(parametroSuperAdmin);

        return comando;
    }
}
