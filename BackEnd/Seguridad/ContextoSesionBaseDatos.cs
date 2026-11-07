namespace AppRobos.Backend.Seguridad;

// Guarda quién está haciendo la petición, para que PostgreSQL aplique la seguridad de filas (RLS).
// Por cada petición hay una instancia (scoped). Debe establecerse antes del primer acceso a la base.
public class ContextoSesionBaseDatos
{
    // Identificador del ciudadano autenticado. Null si la petición no tiene sesión.
    public Guid? UsuarioId { get; private set; }

    // TRUE solo en operaciones de autenticación (login y registro), que necesitan buscar cuentas
    // antes de saber quién es el usuario. Ninguna otra operación debe activarlo.
    public bool ModoSistema { get; private set; }

    // TRUE si la cuenta tiene el rol SuperAdmin en la base de datos. Solo permite leer los reportes de todos.
    public bool EsSuperAdmin { get; private set; }

    // Indica que la petición corresponde al ciudadano con este identificador.
    public void EstablecerUsuario(Guid idUsuario, bool esSuperAdmin = false)
    {
        UsuarioId = idUsuario;
        EsSuperAdmin = esSuperAdmin;
    }

    // Activa el modo sistema para autenticación. Solo lo llaman los servicios de login y registro.
    public void ActivarModoSistema()
    {
        ModoSistema = true;
    }
}
