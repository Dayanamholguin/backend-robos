using Microsoft.AspNetCore.Identity;

namespace AppRobos.Backend.Modelos;

// Cuenta de un ciudadano. Hereda de IdentityUser para usar ASP.NET Core Identity
// (contraseña con hash, correo único, bloqueo y roles).
// Tabla: CuentasUsuario.
public class CuentaUsuario : IdentityUser<Guid>
{
    // Nombre de pila del ciudadano.
    public string Nombre { get; set; } = string.Empty;

    // Apellido del ciudadano.
    public string Apellido { get; set; } = string.Empty;

    // Fecha de nacimiento. Se guarda solo la fecha, sin hora.
    public DateOnly FechaNacimiento { get; set; }

    // Código de sexo/género, validado contra el catálogo SexoGenero.
    public string SexoCodigo { get; set; } = string.Empty;

    // Estado de la cuenta: ACTIVA, INACTIVA o BLOQUEADA.
    // El bloqueo temporal usa LockoutEnd (de Identity) como fecha de fin.
    public string EstadoCodigo { get; set; } = EstadosCuentaCodigos.Activa;

    // Cuántos bloqueos ha tenido la cuenta desde el último login exitoso.
    // Define la duración del siguiente bloqueo (1.er, 2.º, 3.er).
    public int NivelBloqueo { get; set; }

    // TRUE desde el 4.º bloqueo. La cuenta queda bloqueada de forma permanente
    // y solo se reactiva con un cambio directo en la base de datos.
    public bool CuentaDeshabilitada { get; set; }

    // Momento de creación de la cuenta (UTC).
    public DateTimeOffset FechaCreacion { get; set; }

    // Última modificación de la cuenta (UTC).
    public DateTimeOffset FechaActualizacion { get; set; }
}
