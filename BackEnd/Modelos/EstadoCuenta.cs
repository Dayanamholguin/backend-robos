namespace AppRobos.Backend.Modelos;

// Estado posible de una cuenta (ACTIVA, INACTIVA, BLOQUEADA).
// Tabla: EstadosCuenta. Se guarda en BD para poder habilitar o deshabilitar estados sin cambiar código.
public class EstadoCuenta
{
    // Código del estado, por ejemplo ACTIVA.
    public string Codigo { get; set; } = string.Empty;

    // Descripción para quien administre la base de datos.
    public string Descripcion { get; set; } = string.Empty;

    // Si es FALSE, el estado deja de aceptarse sin borrarlo.
    public bool Habilitado { get; set; } = true;
}
