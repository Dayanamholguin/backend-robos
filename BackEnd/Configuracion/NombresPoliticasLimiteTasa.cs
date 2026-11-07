namespace AppRobos.Backend.Configuracion;

// Nombres de las políticas de rate limiting. Deben coincidir con la columna Nombre de PoliticasLimiteTasa.
// Solo se usan las políticas de endpoints que ya existen.
public static class NombresPoliticasLimiteTasa
{
    // Intentos de inicio de sesión. Limita la fuerza bruta por IP.
    public const string Login = "PoliticaLogin";

    // Registro de cuentas nuevas. Limita la creación masiva de cuentas por IP.
    public const string Registro = "PoliticaRegistro";

    // Lectura de reportes. Reservada para los endpoints de reportes.
    public const string LecturaReportes = "PoliticaLecturaReportes";

    // Límite general que se aplica a todas las peticiones de la API.
    public const string PorDefecto = "PoliticaPorDefecto";

    // Todas las políticas que la API reconoce. Una política fuera de esta lista se ignora.
    public static readonly string[] Todos = [Login, Registro, LecturaReportes, PorDefecto];
}
