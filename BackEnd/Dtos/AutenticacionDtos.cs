using System.ComponentModel.DataAnnotations;
using AppRobos.Backend.Validaciones;

namespace AppRobos.Backend.Dtos;

// Datos personales editables de un ciudadano. Los comparten el registro (RF-001) y la edición del perfil (RF-003).
// El correo, el rol y el estado NO están aquí: no son editables por el ciudadano.
// Es una clase (no un record) para que las validaciones vivan en las propiedades.
public abstract class DatosPersonalesDto
{
    // Nombre de pila. Solo letras, espacios, apóstrofes y guiones.
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@"^[\p{L}][\p{L} '\-]*$", ErrorMessage = "El nombre solo puede tener letras, espacios, apóstrofes y guiones.")]
    public string Nombre { get; set; } = string.Empty;

    // Apellido. Mismas reglas que el nombre.
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@"^[\p{L}][\p{L} '\-]*$", ErrorMessage = "El apellido solo puede tener letras, espacios, apóstrofes y guiones.")]
    public string Apellido { get; set; } = string.Empty;

    // Celular de Colombia: 10 dígitos que empiezan por 3.
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^(?!.*(\d)\1{4})3\d{9}$", ErrorMessage = "El teléfono debe tener 10 dígitos, empezar por 3 y no repetir 5 números iguales seguidos (celular de Colombia).")]
    public string Telefono { get; set; } = string.Empty;

    // Fecha de nacimiento. Debe ser una fecha pasada y razonable.
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [FechaNacimientoValida]
    public DateOnly FechaNacimiento { get; set; }

    // Código de sexo/género. Debe existir en el catálogo SexoGenero.
    [Required(ErrorMessage = "El sexo o género es obligatorio.")]
    [StringLength(40)]
    public string SexoCodigo { get; set; } = string.Empty;
}

// Datos que envía el ciudadano para registrarse (RF-001).
public class RegistroUsuarioDto : DatosPersonalesDto
{
    // Correo electrónico. Será el usuario para iniciar sesión y no se puede cambiar después.
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(255)]
    public string Correo { get; set; } = string.Empty;

    // Contraseña. La política de complejidad se aplica en Identity.
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Contrasena { get; set; } = string.Empty;
}

// Datos que envía el ciudadano para editar su perfil (RF-003).
// Reemplaza los datos personales completos. No incluye correo, rol ni estado.
public class ActualizarPerfilDto : DatosPersonalesDto
{
}

// Datos para cambiar la contraseña desde la edición del perfil.
public class CambiarContrasenaDto
{
    // Contraseña actual. Se verifica antes de aceptar el cambio.
    [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
    [StringLength(100)]
    public string ContrasenaActual { get; set; } = string.Empty;

    // Contraseña nueva. Debe cumplir la política y ser distinta de la actual.
    [Required(ErrorMessage = "La contraseña nueva es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña nueva debe tener al menos 8 caracteres.")]
    public string ContrasenaNueva { get; set; } = string.Empty;
}

// Datos que envía el ciudadano para iniciar sesión (RF-002).
public class LoginUsuarioDto
{
    // Correo con el que se registró el ciudadano.
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public string Correo { get; set; } = string.Empty;

    // Contraseña de la cuenta.
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100)]
    public string Contrasena { get; set; } = string.Empty;
}

// Datos de perfil que se devuelven a la app. Nunca incluyen la contraseña ni su hash.
public record PerfilUsuarioDto(
    Guid Id,
    string Correo,
    string Nombre,
    string Apellido,
    string Telefono,
    DateOnly FechaNacimiento,
    string SexoCodigo,
    string Estado,
    IReadOnlyList<string> Roles
);

// Respuesta del login: token JWT, fecha de expiración y perfil del ciudadano.
public record RespuestaLoginDto(
    string Token,
    DateTimeOffset ExpiraEn,
    PerfilUsuarioDto Usuario
);

// Respuesta de la comprobación de correo. Solo indica si el correo está libre; no expone ningún dato de la cuenta.
public record RespuestaCorreoDisponibleDto(bool Disponible);

// Consulta de disponibilidad del teléfono. La regla de formato se valida aquí, igual que en el registro.
public class TelefonoConsultaDto
{
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^(?!.*(\d)\1{4})3\d{9}$", ErrorMessage = "El teléfono debe tener 10 dígitos, empezar por 3 y no repetir 5 números iguales seguidos (celular de Colombia).")]
    public string Telefono { get; set; } = string.Empty;
}

// Respuesta de la comprobación de teléfono. Solo indica si está libre; no expone ningún dato de la cuenta.
public record RespuestaTelefonoDisponibleDto(bool Disponible);
