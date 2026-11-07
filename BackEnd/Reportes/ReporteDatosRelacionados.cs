namespace AppRobos.Backend.Reportes;

// Sección E: datos generales de los presuntos responsables. Opcional. No identifica personas.
// Tabla: ReporteResponsables.
public class ReporteResponsables
{
    public Guid ReporteId { get; set; }
    public string? CantidadResponsables { get; set; }
    public string? ActuaronCoordinados { get; set; }
    public string? MedioHuida { get; set; }
    public string? RostroObservado { get; set; }
    public string? VestimentaObservada { get; set; }
    public string? VehiculoObservado { get; set; }
    public string? TipoVehiculo { get; set; }
    public string? PlacaObservada { get; set; }
    public string? CaracteristicaDistintiva { get; set; }

    public Reporte? Reporte { get; set; }
}

// Sección H: datos de quien reporta. Tabla separada para restringir su acceso.
// La cédula se guarda cifrada y la API no la devuelve.
public class ReporteDatosPersonales
{
    public Guid ReporteId { get; set; }
    public string TipoParticipacion { get; set; } = string.Empty;
    public bool ConsentimientoAceptado { get; set; }
    public DateTimeOffset? FechaConsentimiento { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CedulaCifrada { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;

    public Reporte? Reporte { get; set; }
}

// Características del sector (selección múltiple). Tabla: ReporteCaracteristicaSector.
public class ReporteCaracteristicaSector
{
    public Guid ReporteId { get; set; }
    public string Codigo { get; set; } = string.Empty;

    public Reporte? Reporte { get; set; }
}

// Formas de acceso forzado (selección múltiple). Tabla: ReporteAccesoForzado.
public class ReporteAccesoForzado
{
    public Guid ReporteId { get; set; }
    public string Codigo { get; set; } = string.Empty;

    public Reporte? Reporte { get; set; }
}

// Puntaje del ICR por criterio, guardado al enviar el reporte. Tabla: ReporteIcr.
public class ReporteIcr
{
    public Guid ReporteId { get; set; }
    public string Criterio { get; set; } = string.Empty;
    public decimal Puntaje { get; set; }

    public Reporte? Reporte { get; set; }
}
