using AppRobos.Backend.Dtos;

namespace AppRobos.Backend.Reportes;

// Convierte la entidad del reporte en la respuesta de la API.
// La cédula nunca se devuelve: solo se guarda cifrada y no se muestra.
public static class MapeadorRespuestaReporte
{
    public static ReporteRespuestaDto ADto(Reporte r, IReadOnlyList<string>? avisos = null) => new()
    {
        Id = r.Id,
        Estado = r.EstadoReporte,
        FechaPrimerGuardado = r.FechaPrimerGuardado,
        FechaVencimientoBorrador = r.FechaVencimientoBorrador,
        FechaEnvio = r.FechaEnvio,
        Datos = ADatos(r),
        Icr = r.IcrPuntaje is { } puntaje ? new IcrRespuestaDto
        {
            Puntaje = puntaje,
            Clasificacion = r.IcrClasificacion ?? string.Empty,
            PuntajePorCriterio = r.DetalleIcr.ToDictionary(d => d.Criterio, d => d.Puntaje),
        } : null,
        Aviso = r.EstadoReporte == EstadosReporte.Borrador
            ? "Este borrador se eliminará automáticamente si no lo envías antes de la fecha indicada."
            : null,
    };

    public static ReporteResumenDto AResumen(Reporte r) => new()
    {
        Id = r.Id,
        Estado = r.EstadoReporte,
        FechaIncidente = r.FechaIncidente,
        MunicipioCodigo = r.MunicipioCodigo,
        CategoriaPrincipal = r.CategoriaPrincipal,
        IcrPuntaje = r.IcrPuntaje,
        IcrClasificacion = r.IcrClasificacion,
        FechaVencimientoBorrador = r.FechaVencimientoBorrador,
        FechaEnvio = r.FechaEnvio,
    };

    private static ReporteEntradaDto ADatos(Reporte r) => new()
    {
        FechaIncidente = r.FechaIncidente,
        HoraIncidente = r.HoraIncidente,
        PrecisionTemporal = r.PrecisionTemporal,
        FranjaHoraria = r.FranjaHoraria,
        CondicionClima = r.CondicionClima,
        IntensidadLluvia = r.IntensidadLluvia,
        VisibilidadEntorno = r.VisibilidadEntorno,
        IluminacionLugar = r.IluminacionLugar,
        SuperficieMojada = r.SuperficieMojada,
        AfluenciaPersonas = r.AfluenciaPersonas,
        MunicipioCodigo = r.MunicipioCodigo,
        BarrioId = r.BarrioId,
        TipoUbicacion = r.TipoUbicacion,
        Latitud = r.Latitud,
        Longitud = r.Longitud,
        TipoLugar = r.TipoLugar,
        EntornoLugar = r.EntornoLugar,
        CaracteristicasSector = r.CaracteristicasSector.Select(c => c.Codigo).ToList(),
        TipoIncidente = r.TipoIncidente,
        CategoriaPrincipal = r.CategoriaPrincipal,
        ResultadoHecho = r.ResultadoHecho,
        ContextoIncidente = r.ContextoIncidente,
        RelacionLugar = r.RelacionLugar,
        ModalidadPercibida = r.ModalidadPercibida,
        ContactoFisico = r.ContactoFisico,
        Amenazas = r.Amenazas,
        UsoArma = r.UsoArma,
        TipoArma = r.TipoArma,
        IngresoInmueble = r.IngresoInmueble,
        AccesosForzados = r.AccesosForzados.Select(a => a.Codigo).ToList(),
        SeguimientoPrevio = r.SeguimientoPrevio,
        DistraccionPrevia = r.DistraccionPrevia,
        UsoVehiculo = r.UsoVehiculo,
        MedioAproximacion = r.MedioAproximacion,
        MomentoIntervencion = r.MomentoIntervencion,
        Bienes = r.Bienes.Select(ABien).ToList(),
        Responsables = r.Responsables is null ? null : new ResponsablesEntradaDto
        {
            CantidadResponsables = r.Responsables.CantidadResponsables,
            ActuaronCoordinados = r.Responsables.ActuaronCoordinados,
            MedioHuida = r.Responsables.MedioHuida,
            RostroObservado = r.Responsables.RostroObservado,
            VestimentaObservada = r.Responsables.VestimentaObservada,
            VehiculoObservado = r.Responsables.VehiculoObservado,
            TipoVehiculo = r.Responsables.TipoVehiculo,
            PlacaObservada = r.Responsables.PlacaObservada,
            CaracteristicaDistintiva = r.Responsables.CaracteristicaDistintiva,
        },
        LesionesFisicas = r.LesionesFisicas,
        GravedadPercibida = r.GravedadPercibida,
        AtencionMedica = r.AtencionMedica,
        OtrasPersonasAfectadas = r.OtrasPersonasAfectadas,
        AfectacionMovilidad = r.AfectacionMovilidad,
        DanosMateriales = r.DanosMateriales,
        TipoDanoMaterial = r.TipoDanoMaterial,
        PerdidaAccesoCuentas = r.PerdidaAccesoCuentas,
        InformoAutoridad = r.InformoAutoridad,
        AutoridadCanal = r.AutoridadCanal,
        EstadoDenunciaFormal = r.EstadoDenunciaFormal,
        TieneRadicado = r.TieneRadicado,
        NumeroRadicado = r.NumeroRadicado,
        NotificoBanco = r.NotificoBanco,
        RecuperacionActuacion = r.RecuperacionActuacion,
        // La cédula no se devuelve; solo se muestran los datos de contacto que el ciudadano ya conoce
        DatosPersonales = r.DatosPersonales is null ? null : new DatosPersonalesEntradaDto
        {
            TipoParticipacion = r.DatosPersonales.TipoParticipacion,
            ConsentimientoAceptado = r.DatosPersonales.ConsentimientoAceptado,
            NombreCompleto = r.DatosPersonales.NombreCompleto,
            Correo = r.DatosPersonales.Correo,
            Celular = r.DatosPersonales.Celular,
        },
    };

    private static BienEntradaDto ABien(ReporteBien b) => new()
    {
        CategoriaBien = b.CategoriaBien,
        Cantidad = b.Cantidad,
        ValorAproximado = b.ValorAproximado,
        Recuperado = b.Recuperado,
        InformacionPersonal = b.InformacionPersonal,
        BloqueoReportado = b.BloqueoReportado,
        Celular = b.Celular is null ? null : new CelularEntradaDto
        {
            TipoDispositivo = b.Celular.TipoDispositivo,
            SistemaOperativo = b.Celular.SistemaOperativo,
            EstadoBloqueo = b.Celular.EstadoBloqueo,
            UsoAppsBancarias = b.Celular.UsoAppsBancarias,
        },
        Vehiculo = b.Vehiculo is null ? null : new VehiculoEntradaDto
        {
            TipoVehiculo = b.Vehiculo.TipoVehiculo,
            Estado = b.Vehiculo.Estado,
            Modalidad = b.Vehiculo.Modalidad,
            Recuperacion = b.Vehiculo.Recuperacion,
        },
        Tarjeta = b.Tarjeta is null ? null : new TarjetaEntradaDto
        {
            Tipo = b.Tarjeta.Tipo,
            Estado = b.Tarjeta.Estado,
            TransaccionesNoReconocidas = b.Tarjeta.TransaccionesNoReconocidas,
        },
        Documento = b.Documento is null ? null : new DocumentoEntradaDto
        {
            Tipo = b.Documento.Tipo,
            Estado = b.Documento.Estado,
        },
        ContenidoBolso = b.ContenidoBolso.Select(c => c.Codigo).ToList(),
    };
}
