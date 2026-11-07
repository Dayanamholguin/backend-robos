namespace AppRobos.Backend.Reportes;

// Bien sustraído en un reporte. Un reporte puede tener varios bienes.
// Tabla: ReporteBien. Las características propias de cada categoría van en tablas hijas.
public class ReporteBien
{
    public Guid Id { get; set; }
    public Guid ReporteId { get; set; }

    public string CategoriaBien { get; set; } = string.Empty;
    public string Cantidad { get; set; } = string.Empty;
    public string ValorAproximado { get; set; } = string.Empty;
    public string? Recuperado { get; set; }
    public string? InformacionPersonal { get; set; }
    public string? BloqueoReportado { get; set; }

    public Reporte? Reporte { get; set; }
    public ReporteBienCelular? Celular { get; set; }
    public ReporteBienVehiculo? Vehiculo { get; set; }
    public ReporteBienTarjeta? Tarjeta { get; set; }
    public ReporteBienDocumento? Documento { get; set; }
    public ICollection<ReporteBienContenidoBolso> ContenidoBolso { get; set; } = new List<ReporteBienContenidoBolso>();
}
