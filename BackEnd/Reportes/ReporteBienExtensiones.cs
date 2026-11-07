namespace AppRobos.Backend.Reportes;

// Características específicas de un celular. Solo existe si el bien es de categoría CELULAR.
// Tabla: ReporteBienCelular.
public class ReporteBienCelular
{
    public Guid ReporteBienId { get; set; }
    public string? TipoDispositivo { get; set; }
    public string? SistemaOperativo { get; set; }
    public string? EstadoBloqueo { get; set; }
    public string? UsoAppsBancarias { get; set; }

    public ReporteBien? Bien { get; set; }
}

// Características específicas de un vehículo. Solo existe si el bien es un vehículo.
// Tabla: ReporteBienVehiculo.
public class ReporteBienVehiculo
{
    public Guid ReporteBienId { get; set; }
    public string? TipoVehiculo { get; set; }
    public string? Estado { get; set; }
    public string? Modalidad { get; set; }
    public string? Recuperacion { get; set; }

    public ReporteBien? Bien { get; set; }
}

// Características de una tarjeta bancaria. Tabla: ReporteBienTarjeta.
public class ReporteBienTarjeta
{
    public Guid ReporteBienId { get; set; }
    public string? Tipo { get; set; }
    public string? Estado { get; set; }
    public string? TransaccionesNoReconocidas { get; set; }

    public ReporteBien? Bien { get; set; }
}

// Características de documentos personales. Tabla: ReporteBienDocumento.
public class ReporteBienDocumento
{
    public Guid ReporteBienId { get; set; }
    public string? Tipo { get; set; }
    public string? Estado { get; set; }

    public ReporteBien? Bien { get; set; }
}

// Contenido de un bolso o cartera. Un bolso puede tener varias opciones.
// Tabla: ReporteBienContenidoBolso.
public class ReporteBienContenidoBolso
{
    public Guid ReporteBienId { get; set; }
    public string Codigo { get; set; } = string.Empty;

    public ReporteBien? Bien { get; set; }
}
