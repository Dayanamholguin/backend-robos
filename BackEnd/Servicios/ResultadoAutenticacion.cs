namespace AppRobos.Backend.Servicios;

// Tipo de resultado de una operación de autenticación. El controlador lo traduce a HTTP.
public enum EstadoResultadoAuth
{
    // La operación se completó.
    Exitoso,

    // Los datos enviados no son válidos (400).
    Invalido,

    // El correo ya existe (409).
    Conflicto,

    // Correo o contraseña incorrectos (401). No se revela cuál de los dos falló.
    CredencialesInvalidas,

    // La cuenta está bloqueada temporalmente (423).
    Bloqueada,

    // La cuenta está deshabilitada de forma permanente (403).
    Deshabilitada,

    // La cuenta está inactiva (403).
    Inactiva
}

// Resultado de una operación de autenticación con sus datos o mensajes.
// Tipo de dato que devuelve la operación cuando es exitosa.
public record ResultadoAuth<T>(
    EstadoResultadoAuth Estado,
    T? Datos = default,
    IReadOnlyList<string>? Mensajes = null,
    DateTimeOffset? BloqueadoHasta = null)
{
    // Operación completada con datos.
    public static ResultadoAuth<T> Exito(T datos) => new(EstadoResultadoAuth.Exitoso, datos);

    // Datos no válidos, con la lista de mensajes para el ciudadano.
    public static ResultadoAuth<T> Invalido(IEnumerable<string> mensajes) =>
        new(EstadoResultadoAuth.Invalido, default, mensajes.ToList());

    // Estado sin datos adicionales (credenciales, deshabilitada, inactiva, conflicto).
    public static ResultadoAuth<T> Sin(EstadoResultadoAuth estado, string mensaje) =>
        new(estado, default, [mensaje]);

    // Cuenta bloqueada temporalmente hasta una fecha.
    public static ResultadoAuth<T> Bloqueo(DateTimeOffset hasta) =>
        new(EstadoResultadoAuth.Bloqueada, default, null, hasta);
}
