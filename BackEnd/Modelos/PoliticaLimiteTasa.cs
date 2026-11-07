namespace AppRobos.Backend.Modelos;

// Política de rate limiting guardada en la base de datos (tabla PoliticasLimiteTasa).
// La API carga estos valores en memoria y los recarga cada 60 segundos, sin consultar la BD en cada petición.
public class PoliticaLimiteTasa
{
    // Identificador interno.
    public int Id { get; set; }

    // Nombre único de la política, por ejemplo PoliticaLogin.
    public string Nombre { get; set; } = string.Empty;

    // Peticiones permitidas dentro de cada ventana, por IP.
    public int LimitePeticiones { get; set; }

    // Duración de la ventana fija en segundos.
    public int VentanaSegundos { get; set; }

    // Cómo se agrupan las peticiones. Solo se admite IP.
    public string EstrategiaParticion { get; set; } = "IP";

    // Tamaño de la cola de espera. Siempre 0: el exceso se rechaza de inmediato.
    public int LimiteCola { get; set; }

    // Si es FALSE, la política no se aplica. Login y registro no se pueden desactivar.
    public bool Activa { get; set; } = true;

    // Momento de creación (UTC).
    public DateTimeOffset FechaCreacion { get; set; }

    // Última modificación (UTC).
    public DateTimeOffset FechaActualizacion { get; set; }
}
