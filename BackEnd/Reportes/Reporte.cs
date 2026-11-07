namespace AppRobos.Backend.Reportes;

// Reporte ciudadano de un hurto. Puede estar en BORRADOR (editable) o ENVIADO (inmutable).
// Tabla: Reporte. Los campos categóricos guardan el código de la opción elegida en CatalogoOpciones.
public class Reporte
{
    public Guid Id { get; set; }

    // Autor del reporte. Solo el autor ve, edita o borra un borrador.
    public Guid UsuarioId { get; set; }

    // BORRADOR o ENVIADO.
    public string EstadoReporte { get; set; } = EstadosReporte.Borrador;

    // Primer guardado parcial. El plazo de 15 días no se extiende con guardados posteriores.
    public DateTimeOffset FechaPrimerGuardado { get; set; }

    // Fecha en que el borrador se elimina si no se envía. NULL cuando el reporte se envía.
    public DateTimeOffset? FechaVencimientoBorrador { get; set; }

    // Fecha de envío, asignada por el servidor.
    public DateTimeOffset? FechaEnvio { get; set; }

    public DateTimeOffset FechaActualizacion { get; set; }

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

    // Coordenadas. No se guarda la precisión del GPS. La columna Ubicacion (geography) la calcula la base.
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }

    public string? TipoLugar { get; set; }
    public string? EntornoLugar { get; set; }

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
    public string? SeguimientoPrevio { get; set; }
    public string? DistraccionPrevia { get; set; }
    public string? UsoVehiculo { get; set; }
    public string? MedioAproximacion { get; set; }
    public string? MomentoIntervencion { get; set; }

    // Sección F: consecuencias y afectación
    public string? LesionesFisicas { get; set; }
    public string? GravedadPercibida { get; set; }
    public string? AtencionMedica { get; set; }
    public string? OtrasPersonasAfectadas { get; set; }
    public string? AfectacionMovilidad { get; set; }
    public string? DanosMateriales { get; set; }
    public string? TipoDanoMaterial { get; set; }
    public string? PerdidaAccesoCuentas { get; set; }

    // Sección G: denuncia y radicado
    public string? InformoAutoridad { get; set; }
    public string? AutoridadCanal { get; set; }
    public string? EstadoDenunciaFormal { get; set; }
    public string? TieneRadicado { get; set; }
    public string? NumeroRadicado { get; set; }
    public string? NotificoBanco { get; set; }
    public string? RecuperacionActuacion { get; set; }

    // Resultado del ICR, calculado al enviar
    public decimal? IcrPuntaje { get; set; }
    public string? IcrClasificacion { get; set; }

    // Relaciones
    public ICollection<ReporteBien> Bienes { get; set; } = new List<ReporteBien>();
    public ICollection<ReporteCaracteristicaSector> CaracteristicasSector { get; set; } = new List<ReporteCaracteristicaSector>();
    public ICollection<ReporteAccesoForzado> AccesosForzados { get; set; } = new List<ReporteAccesoForzado>();
    public ReporteResponsables? Responsables { get; set; }
    public ReporteDatosPersonales? DatosPersonales { get; set; }
    public ICollection<ReporteIcr> DetalleIcr { get; set; } = new List<ReporteIcr>();
}

// Estados posibles de un reporte.
public static class EstadosReporte
{
    public const string Borrador = "BORRADOR";
    public const string Enviado = "ENVIADO";
}
