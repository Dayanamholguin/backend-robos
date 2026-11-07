namespace AppRobos.Backend.Modelos;

// Criterio del ICR con su peso dentro de los 100 puntos. Tabla: CriteriosIcr.
// La suma de los criterios activos debe ser 100; la base de datos lo valida.
public class CriterioIcr
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public bool Activo { get; set; } = true;
}
