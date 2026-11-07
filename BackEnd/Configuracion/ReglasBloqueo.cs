using System.ComponentModel.DataAnnotations;

namespace AppRobos.Backend.Configuracion;

// Reglas del bloqueo escalonado de cuentas por intentos fallidos de login.
// Se leen de la sección "ReglasBloqueo". Migrar a la tabla ConfiguracionSistema cuando exista.
public class ReglasBloqueo
{
    // Nombre de la sección en appsettings o user-secrets.
    public const string NombreSeccion = "ReglasBloqueo";

    // Intentos fallidos seguidos que disparan un bloqueo.
    [Range(1, 10, ErrorMessage = "Los intentos máximos deben estar entre 1 y 10.")]
    public int MaximoIntentosFallidos { get; set; } = 3;

    // Duración en horas de cada bloqueo temporal, en orden: 1.er bloqueo, 2.º, 3.er.
    // Desde el siguiente bloqueo (el 4.º) la cuenta queda deshabilitada de forma permanente.
    [MinLength(1, ErrorMessage = "Debe existir al menos una duración de bloqueo.")]
    public int[] DuracionesHoras { get; set; } = [3, 24, 168];
}
