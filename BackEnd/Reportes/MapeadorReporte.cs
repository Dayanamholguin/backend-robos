using AppRobos.Backend.Dtos;

namespace AppRobos.Backend.Reportes;

// Pasa los datos del DTO a la entidad y de la entidad al DTO de respuesta.
// Al guardar, borra los campos que no aplican según las respuestas anteriores.
public static class MapeadorReporte
{
    // Categorías que tienen datos propios en una tabla hija.
    private static readonly string[] CategoriasVehiculo = ["AUTOMOVIL", "MOTOCICLETA", "BICICLETA"];

    // Ubicación de Colombia (UTC-5, sin cambio de horario). Las fechas del formulario son hora local.
    public static readonly TimeSpan DesfaseLocal = TimeSpan.FromHours(-5);

    // Aplica el DTO a la entidad. El cifrado de la cédula se recibe como función para no depender de la infraestructura.
    public static void Aplicar(ReporteEntradaDto dto, Reporte reporte, Func<string, string> cifrarCedula)
    {
        AplicarSeccionA(dto, reporte);
        AplicarSeccionBC(dto, reporte);
        AplicarBienes(dto, reporte);
        AplicarResponsables(dto, reporte);
        AplicarSeccionFG(dto, reporte);
        AplicarDatosPersonales(dto, reporte, cifrarCedula);
    }

    // Franja horaria calculada a partir de la hora exacta.
    public static string? FranjaDeHora(TimeOnly? hora) => hora switch
    {
        null => null,
        { Hour: < 6 } => "MADRUGADA",
        { Hour: < 12 } => "MANANA",
        { Hour: < 18 } => "TARDE",
        _ => "NOCHE",
    };

    // Momento del incidente en UTC, a partir de la fecha y la hora local. Null si falta la hora exacta.
    public static DateTimeOffset? MomentoIncidenteUtc(Reporte reporte)
    {
        if (reporte.FechaIncidente is not { } fecha || reporte.HoraIncidente is not { } hora || reporte.PrecisionTemporal != "HORA_EXACTA")
        {
            return null;
        }

        var local = new DateTimeOffset(fecha.ToDateTime(hora), DesfaseLocal);
        return local.ToUniversalTime();
    }

    private static void AplicarSeccionA(ReporteEntradaDto dto, Reporte reporte)
    {
        reporte.FechaIncidente = dto.FechaIncidente;
        reporte.PrecisionTemporal = dto.PrecisionTemporal;

        // La hora solo existe cuando es exacta; la franja se calcula, salvo que la precisión sea franja aproximada
        reporte.HoraIncidente = dto.PrecisionTemporal == "HORA_EXACTA" ? dto.HoraIncidente : null;
        reporte.FranjaHoraria = dto.PrecisionTemporal switch
        {
            "HORA_EXACTA" => FranjaDeHora(reporte.HoraIncidente),
            "FRANJA_APROXIMADA" => dto.FranjaHoraria,
            _ => null,
        };

        reporte.CondicionClima = dto.CondicionClima;
        reporte.IntensidadLluvia = dto.IntensidadLluvia;
        reporte.VisibilidadEntorno = dto.VisibilidadEntorno;
        reporte.IluminacionLugar = dto.IluminacionLugar;
        reporte.SuperficieMojada = dto.SuperficieMojada;
        reporte.AfluenciaPersonas = dto.AfluenciaPersonas;
        reporte.MunicipioCodigo = dto.MunicipioCodigo;
        reporte.BarrioId = dto.BarrioId;
        reporte.TipoUbicacion = dto.TipoUbicacion;
        reporte.Latitud = dto.Latitud;
        reporte.Longitud = dto.Longitud;
        reporte.TipoLugar = dto.TipoLugar;
        reporte.EntornoLugar = dto.EntornoLugar;

        reporte.CaracteristicasSector = (dto.CaracteristicasSector ?? [])
            .Distinct()
            .Select(codigo => new ReporteCaracteristicaSector { ReporteId = reporte.Id, Codigo = codigo })
            .ToList();
    }

    private static void AplicarSeccionBC(ReporteEntradaDto dto, Reporte reporte)
    {
        reporte.TipoIncidente = dto.TipoIncidente;
        reporte.CategoriaPrincipal = dto.CategoriaPrincipal;
        reporte.ResultadoHecho = dto.ResultadoHecho;
        reporte.ContextoIncidente = dto.ContextoIncidente;
        reporte.RelacionLugar = dto.RelacionLugar;

        reporte.ModalidadPercibida = dto.ModalidadPercibida;
        reporte.ContactoFisico = dto.ContactoFisico;
        reporte.Amenazas = dto.Amenazas;
        reporte.UsoArma = dto.UsoArma;
        // El tipo de arma solo aplica si hubo arma
        reporte.TipoArma = dto.UsoArma == "SI" ? dto.TipoArma : null;
        reporte.IngresoInmueble = dto.IngresoInmueble;
        reporte.SeguimientoPrevio = dto.SeguimientoPrevio;
        reporte.DistraccionPrevia = dto.DistraccionPrevia;
        reporte.UsoVehiculo = dto.UsoVehiculo;
        reporte.MedioAproximacion = dto.MedioAproximacion;
        reporte.MomentoIntervencion = dto.MomentoIntervencion;

        reporte.AccesosForzados = (dto.AccesosForzados ?? [])
            .Distinct()
            .Select(codigo => new ReporteAccesoForzado { ReporteId = reporte.Id, Codigo = codigo })
            .ToList();
    }

    private static void AplicarBienes(ReporteEntradaDto dto, Reporte reporte)
    {
        reporte.Bienes = (dto.Bienes ?? [])
            .Select(bienDto => CrearBien(bienDto, reporte.Id))
            .ToList();
    }

    // Crea un bien con solo los datos propios de su categoría.
    private static ReporteBien CrearBien(BienEntradaDto dto, Guid reporteId)
    {
        var bien = new ReporteBien
        {
            Id = Guid.NewGuid(),
            ReporteId = reporteId,
            CategoriaBien = dto.CategoriaBien ?? string.Empty,
            Cantidad = dto.Cantidad ?? string.Empty,
            ValorAproximado = dto.ValorAproximado ?? string.Empty,
            Recuperado = dto.Recuperado,
            InformacionPersonal = dto.InformacionPersonal,
            BloqueoReportado = dto.BloqueoReportado,
        };

        if (bien.CategoriaBien == "CELULAR" && dto.Celular is not null)
        {
            bien.Celular = new ReporteBienCelular
            {
                ReporteBienId = bien.Id,
                TipoDispositivo = dto.Celular.TipoDispositivo,
                SistemaOperativo = dto.Celular.SistemaOperativo,
                EstadoBloqueo = dto.Celular.EstadoBloqueo,
                UsoAppsBancarias = dto.Celular.UsoAppsBancarias,
            };
        }

        if (CategoriasVehiculo.Contains(bien.CategoriaBien) && dto.Vehiculo is not null)
        {
            bien.Vehiculo = new ReporteBienVehiculo
            {
                ReporteBienId = bien.Id,
                TipoVehiculo = dto.Vehiculo.TipoVehiculo,
                Estado = dto.Vehiculo.Estado,
                Modalidad = dto.Vehiculo.Modalidad,
                Recuperacion = dto.Vehiculo.Recuperacion,
            };
        }

        if (bien.CategoriaBien == "TARJETA_BANCARIA" && dto.Tarjeta is not null)
        {
            bien.Tarjeta = new ReporteBienTarjeta
            {
                ReporteBienId = bien.Id,
                Tipo = dto.Tarjeta.Tipo,
                Estado = dto.Tarjeta.Estado,
                TransaccionesNoReconocidas = dto.Tarjeta.TransaccionesNoReconocidas,
            };
        }

        if (bien.CategoriaBien == "DOCUMENTOS_PERSONALES" && dto.Documento is not null)
        {
            bien.Documento = new ReporteBienDocumento
            {
                ReporteBienId = bien.Id,
                Tipo = dto.Documento.Tipo,
                Estado = dto.Documento.Estado,
            };
        }

        if (bien.CategoriaBien == "BOLSO_MORRAL_CARTERA")
        {
            bien.ContenidoBolso = (dto.ContenidoBolso ?? [])
                .Distinct()
                .Select(codigo => new ReporteBienContenidoBolso { ReporteBienId = bien.Id, Codigo = codigo })
                .ToList();
        }

        return bien;
    }

    private static void AplicarResponsables(ReporteEntradaDto dto, Reporte reporte)
    {
        if (dto.Responsables is null)
        {
            reporte.Responsables = null;
            return;
        }

        var responsables = dto.Responsables;
        var vehiculoVisto = responsables.VehiculoObservado == "SI";

        reporte.Responsables = new ReporteResponsables
        {
            ReporteId = reporte.Id,
            CantidadResponsables = responsables.CantidadResponsables,
            ActuaronCoordinados = responsables.ActuaronCoordinados,
            MedioHuida = responsables.MedioHuida,
            RostroObservado = responsables.RostroObservado,
            VestimentaObservada = responsables.VestimentaObservada,
            VehiculoObservado = responsables.VehiculoObservado,
            // El tipo de vehículo solo aplica si se observó un vehículo
            TipoVehiculo = vehiculoVisto ? responsables.TipoVehiculo : null,
            PlacaObservada = responsables.PlacaObservada,
            CaracteristicaDistintiva = responsables.CaracteristicaDistintiva,
        };
    }

    private static void AplicarSeccionFG(ReporteEntradaDto dto, Reporte reporte)
    {
        reporte.LesionesFisicas = dto.LesionesFisicas;
        reporte.GravedadPercibida = dto.GravedadPercibida;
        reporte.AtencionMedica = dto.AtencionMedica;
        reporte.OtrasPersonasAfectadas = dto.OtrasPersonasAfectadas;
        reporte.AfectacionMovilidad = dto.AfectacionMovilidad;
        reporte.DanosMateriales = dto.DanosMateriales;
        reporte.TipoDanoMaterial = dto.DanosMateriales == "SI" ? dto.TipoDanoMaterial : null;
        reporte.PerdidaAccesoCuentas = dto.PerdidaAccesoCuentas;

        // Denuncia y radicado: el número solo existe si la respuesta es SI
        reporte.InformoAutoridad = dto.InformoAutoridad;
        var mostrarDenuncia = dto.InformoAutoridad is "SI" or "EN_PROCESO";
        reporte.AutoridadCanal = mostrarDenuncia ? dto.AutoridadCanal : null;
        reporte.EstadoDenunciaFormal = mostrarDenuncia ? dto.EstadoDenunciaFormal : null;
        reporte.TieneRadicado = dto.TieneRadicado;
        reporte.NumeroRadicado = dto.TieneRadicado == "SI" ? dto.NumeroRadicado?.Trim() : null;
        reporte.NotificoBanco = dto.NotificoBanco;
        reporte.RecuperacionActuacion = dto.RecuperacionActuacion;
    }

    private static void AplicarDatosPersonales(ReporteEntradaDto dto, Reporte reporte, Func<string, string> cifrarCedula)
    {
        if (dto.DatosPersonales is null)
        {
            reporte.DatosPersonales = null;
            return;
        }

        var datos = dto.DatosPersonales;
        var existente = reporte.DatosPersonales;

        reporte.DatosPersonales = new ReporteDatosPersonales
        {
            ReporteId = reporte.Id,
            TipoParticipacion = datos.TipoParticipacion ?? string.Empty,
            ConsentimientoAceptado = datos.ConsentimientoAceptado,
            FechaConsentimiento = datos.ConsentimientoAceptado ? DateTimeOffset.UtcNow : null,
            NombreCompleto = datos.NombreCompleto?.Trim() ?? string.Empty,
            // Si la app no reenvía la cédula en una actualización, se conserva la cifrada que ya existe
            CedulaCifrada = string.IsNullOrWhiteSpace(datos.Cedula)
                ? existente?.CedulaCifrada ?? string.Empty
                : cifrarCedula(datos.Cedula.Trim()),
            Correo = datos.Correo?.Trim().ToLowerInvariant() ?? string.Empty,
            Celular = datos.Celular?.Trim() ?? string.Empty,
        };
    }
}
