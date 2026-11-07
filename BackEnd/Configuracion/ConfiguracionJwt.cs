using System.ComponentModel.DataAnnotations;

namespace AppRobos.Backend.Configuracion;

// Parámetros para firmar y validar los tokens JWT.
// Se leen de la sección "ConfiguracionJwt" de la configuración.
public class ConfiguracionJwt
{
    // Nombre de la sección en appsettings o user-secrets.
    public const string NombreSeccion = "ConfiguracionJwt";

    // Clave secreta para firmar los tokens (mínimo 32 caracteres).
    // Debe venir de user-secrets o de variables de entorno, nunca del repositorio.
    [Required(ErrorMessage = "Falta la clave JWT (ConfiguracionJwt:Clave).")]
    [MinLength(32, ErrorMessage = "La clave JWT debe tener al menos 32 caracteres.")]
    public string Clave { get; set; } = string.Empty;

    // Quién emite el token.
    [Required]
    public string Emisor { get; set; } = "AppRobos.Backend";

    // Para quién es el token (la aplicación móvil).
    [Required]
    public string Audiencia { get; set; } = "AppRobos.App";

    // Minutos de vida del token. Corto a propósito para limitar el daño si se filtra.
    [Range(5, 1440, ErrorMessage = "La duración del token debe estar entre 5 y 1440 minutos.")]
    public int MinutosExpiracion { get; set; } = 60;
}
