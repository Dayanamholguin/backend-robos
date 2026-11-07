using System.Text.Json.Serialization;

namespace AppRobos.Backend.Dtos;

// Datos del reporte que envía la app. Todos los campos son opcionales para permitir guardados parciales.
// Al enviar, el servidor revisa que estén los campos obligatorios.
public class ReporteEntradaDto
{
    // Sección A: fecha, hora y ubicación
    public DateOnly? FechaIncidente { get; set; }
    public TimeOnly? HoraIncidente { get; set; }
    public string? PrecisionTemporal { get; set; }
    public string? FranjaHoraria { get; set; }
    public string? CondicionClima { get; set; }
    public string? IntensidadLluvia { get; set; }
    public string? VisibilidadEntorno { get; set; }
    public string? IluminacionLugar { get; set; }
    public string? SuperficieMojada { get; set; }
    public string? AfluenciaPersonas { get; set; }
    public string? MunicipioCodigo { get; set; }
    public int? BarrioId { get; set; }
    public string? TipoUbicacion { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public string? TipoLugar { get; set; }
    public string? EntornoLugar { get; set; }
    public List<string>? CaracteristicasSector { get; set; }

    // Sección B: tipo y estado del incidente
    public string? TipoIncidente { get; set; }
    public string? CategoriaPrincipal { get; set; }
    public string? ResultadoHecho { get; set; }
    public string? ContextoIncidente { get; set; }
    public string? RelacionLugar { get; set; }

    // Sección C: modalidad y circunstancias
    public string? ModalidadPercibida { get; set; }
    public string? ContactoFisico { get; set; }
    public string? Amenazas { get; set; }
    public string? UsoArma { get; set; }
    public string? TipoArma { get; set; }
    public string? IngresoInmueble { get; set; }
    public List<string>? AccesosForzados { get; set; }
    public string? SeguimientoPrevio { get; set; }
    public string? DistraccionPrevia { get; set; }
    public string? UsoVehiculo { get; set; }
    public string? MedioAproximacion { get; set; }
    public string? MomentoIntervencion { get; set; }

    // Sección D: bienes sustraídos
    public List<BienEntradaDto>? Bienes { get; set; }

    // Sección E: presuntos responsables (opcional)
    public ResponsablesEntradaDto? Responsables { get; set; }

    // Sección F: consecuencias y afectación
    public string? LesionesFisicas { get; set; }
    public string? GravedadPercibida { get; set; }
    public string? AtencionMedica { get; set; }
    public string? OtrasPersonasAfectadas { get; set; }
    public string? AfectacionMovilidad { get; set; }
    public string? DanosMateriales { get; set; }
    public string? TipoDanoMaterial { get; set; }
    public string? PerdidaAccesoCuentas { get; set; }

    // Sección G: denuncia y radicado (opcional)
    public string? InformoAutoridad { get; set; }
    public string? AutoridadCanal { get; set; }
    public string? EstadoDenunciaFormal { get; set; }
    public string? TieneRadicado { get; set; }
    public string? NumeroRadicado { get; set; }
    public string? NotificoBanco { get; set; }
    public string? RecuperacionActuacion { get; set; }

    // Sección H: datos de quien reporta
    public DatosPersonalesEntradaDto? DatosPersonales { get; set; }
}

// Un bien sustraído con sus datos generales y los específicos de su categoría.
public class BienEntradaDto
{
    public string? CategoriaBien { get; set; }
    public string? Cantidad { get; set; }
    public string? ValorAproximado { get; set; }
    public string? Recuperado { get; set; }
    public string? InformacionPersonal { get; set; }
    public string? BloqueoReportado { get; set; }

    public CelularEntradaDto? Celular { get; set; }
    public VehiculoEntradaDto? Vehiculo { get; set; }
    public TarjetaEntradaDto? Tarjeta { get; set; }
    public DocumentoEntradaDto? Documento { get; set; }
    public List<string>? ContenidoBolso { get; set; }
}

public class CelularEntradaDto
{
    public string? TipoDispositivo { get; set; }
    public string? SistemaOperativo { get; set; }
    public string? EstadoBloqueo { get; set; }
    public string? UsoAppsBancarias { get; set; }
}

public class VehiculoEntradaDto
{
    public string? TipoVehiculo { get; set; }
    public string? Estado { get; set; }
    public string? Modalidad { get; set; }
    public string? Recuperacion { get; set; }
}

public class TarjetaEntradaDto
{
    public string? Tipo { get; set; }
    public string? Estado { get; set; }
    public string? TransaccionesNoReconocidas { get; set; }
}

public class DocumentoEntradaDto
{
    public string? Tipo { get; set; }
    public string? Estado { get; set; }
}

// Sección E: datos generales de los presuntos responsables. No identifica personas.
public class ResponsablesEntradaDto
{
    public string? CantidadResponsables { get; set; }
    public string? ActuaronCoordinados { get; set; }
    public string? MedioHuida { get; set; }
    public string? RostroObservado { get; set; }
    public string? VestimentaObservada { get; set; }
    public string? VehiculoObservado { get; set; }
    public string? TipoVehiculo { get; set; }
    public string? PlacaObservada { get; set; }
    public string? CaracteristicaDistintiva { get; set; }
}

// Sección H: datos de quien reporta. La cédula solo se recibe; nunca se devuelve.
public class DatosPersonalesEntradaDto
{
    public string? TipoParticipacion { get; set; }
    public bool ConsentimientoAceptado { get; set; }
    public string? NombreCompleto { get; set; }
    // La cédula solo se recibe; si no hay valor no se escribe en la respuesta
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Cedula { get; set; }
    public string? Correo { get; set; }
    public string? Celular { get; set; }
}

// Respuesta de un reporte. La cédula no aparece.
public class ReporteRespuestaDto
{
    public Guid Id { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTimeOffset FechaPrimerGuardado { get; set; }
    public DateTimeOffset? FechaVencimientoBorrador { get; set; }
    public DateTimeOffset? FechaEnvio { get; set; }
    public ReporteEntradaDto Datos { get; set; } = new();
    public IcrRespuestaDto? Icr { get; set; }
    public string? Aviso { get; set; }
}

// Resultado del ICR devuelto al enviar el reporte.
public class IcrRespuestaDto
{
    public decimal Puntaje { get; set; }
    public string Clasificacion { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, decimal> PuntajePorCriterio { get; set; } = new Dictionary<string, decimal>();
}

// Opción de una lista desplegable para la app.
public class OpcionCatalogoRespuestaDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = string.Empty;
    public int Orden { get; set; }
}

// Barrio o sector para la lista dependiente del municipio.
public class BarrioRespuestaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string MunicipioCodigo { get; set; } = string.Empty;
}

// Resumen de un reporte para la lista del ciudadano.
public class ReporteResumenDto
{
    public Guid Id { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateOnly? FechaIncidente { get; set; }
    public string? MunicipioCodigo { get; set; }
    public string? CategoriaPrincipal { get; set; }
    public decimal? IcrPuntaje { get; set; }
    public string? IcrClasificacion { get; set; }
    public DateTimeOffset? FechaVencimientoBorrador { get; set; }
    public DateTimeOffset? FechaEnvio { get; set; }
}
