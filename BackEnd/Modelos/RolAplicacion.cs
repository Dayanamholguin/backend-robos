using Microsoft.AspNetCore.Identity;

namespace AppRobos.Backend.Modelos;

// Rol de la aplicación (solo Ciudadano). Tabla: RolesAplicacion.
public class RolAplicacion : IdentityRole<Guid>
{
}
