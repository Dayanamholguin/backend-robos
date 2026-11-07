using AppRobos.Backend.Icr;

namespace AppRobos.Backend.Reportes;

// Revisa cada campo visible del reporte y dice si tiene un dato que cuente para la completitud.
// Las respuestas que no suman (no sabe, otro, no aplica...) se marcan como sin dato.
// Los campos ocultos por una respuesta anterior no se incluyen.
public static class ConstructorCamposCompletitud
{
    // Categorías de bien que muestran campos específicos.
    private static readonly string[] CategoriasVehiculo = ["AUTOMOVIL", "MOTOCICLETA", "BICICLETA"];

    // esDato(catalogo, codigo) devuelve TRUE si el código existe y cuenta como dato.
    public static IReadOnlyList<CampoCompletitud> Generar(Reporte reporte, Func<string, string?, bool> esDato)
    {
        var campos = new List<CampoCompletitud>();

        // Campos relevantes: los que más pesan en el análisis
        campos.Add(new CampoCompletitud("FechaIncidente", true, reporte.FechaIncidente is not null));
        campos.Add(new CampoCompletitud("HoraOFranja", true, TieneHoraOFranja(reporte, esDato)));
        campos.Add(new CampoCompletitud("Municipio", true, reporte.MunicipioCodigo is not null));
        campos.Add(new CampoCompletitud("Barrio", true, reporte.BarrioId is not null));
        campos.Add(new CampoCompletitud("Ubicacion", true, reporte.Latitud is not null && reporte.Longitud is not null));
        campos.Add(new CampoCompletitud("TipoIncidente", true, esDato("TipoIncidente", reporte.TipoIncidente)));
        campos.Add(new CampoCompletitud("CategoriaPrincipal", true, esDato("CategoriaPrincipal", reporte.CategoriaPrincipal)));
        campos.Add(new CampoCompletitud("ModalidadPercibida", true, esDato("ModalidadPercibida", reporte.ModalidadPercibida)));
        campos.Add(new CampoCompletitud("UsoArma", true, esDato("SiNoNoSabe", reporte.UsoArma)));
        campos.Add(new CampoCompletitud("Bienes", true, reporte.Bienes.Any(b => BienCompleto(b, esDato))));

        // Campos secundarios de las secciones A, B, C y F
        AgregarSecundario(campos, "TipoUbicacion", esDato("TipoUbicacion", reporte.TipoUbicacion));
        AgregarSecundario(campos, "TipoLugar", esDato("TipoLugar", reporte.TipoLugar));
        AgregarSecundario(campos, "EntornoLugar", esDato("EntornoLugar", reporte.EntornoLugar));
        AgregarSecundario(campos, "CondicionClima", esDato("CondicionClima", reporte.CondicionClima));
        AgregarSecundario(campos, "IntensidadLluvia", esDato("IntensidadLluvia", reporte.IntensidadLluvia));
        AgregarSecundario(campos, "VisibilidadEntorno", esDato("VisibilidadEntorno", reporte.VisibilidadEntorno));
        AgregarSecundario(campos, "IluminacionLugar", esDato("IluminacionLugar", reporte.IluminacionLugar));
        AgregarSecundario(campos, "SuperficieMojada", esDato("SiNoNoSabe", reporte.SuperficieMojada));
        AgregarSecundario(campos, "AfluenciaPersonas", esDato("AfluenciaPersonas", reporte.AfluenciaPersonas));
        AgregarSecundario(campos, "ResultadoHecho", esDato("ResultadoHecho", reporte.ResultadoHecho));
        AgregarSecundario(campos, "ContextoIncidente", esDato("ContextoIncidente", reporte.ContextoIncidente));
        AgregarSecundario(campos, "RelacionLugar", esDato("RelacionLugar", reporte.RelacionLugar));
        AgregarSecundario(campos, "ContactoFisico", esDato("SiNoNoSabe", reporte.ContactoFisico));
        AgregarSecundario(campos, "Amenazas", esDato("SiNoNoSabe", reporte.Amenazas));
        AgregarSecundario(campos, "IngresoInmueble", esDato("IngresoInmueble", reporte.IngresoInmueble));
        AgregarSecundario(campos, "AccesosForzados", reporte.AccesosForzados.Any(a => esDato("AccesoForzado", a.Codigo)));
        AgregarSecundario(campos, "SeguimientoPrevio", esDato("SiNoNoSabe", reporte.SeguimientoPrevio));
        AgregarSecundario(campos, "DistraccionPrevia", esDato("SiNoNoSabe", reporte.DistraccionPrevia));
        AgregarSecundario(campos, "UsoVehiculo", esDato("SiNoNoSabe", reporte.UsoVehiculo));
        AgregarSecundario(campos, "MedioAproximacion", esDato("MedioAproximacion", reporte.MedioAproximacion));
        AgregarSecundario(campos, "MomentoIntervencion", esDato("MomentoIntervencion", reporte.MomentoIntervencion));
        AgregarSecundario(campos, "LesionesFisicas", esDato("LesionesFisicas", reporte.LesionesFisicas));
        AgregarSecundario(campos, "GravedadPercibida", esDato("GravedadPercibida", reporte.GravedadPercibida));
        AgregarSecundario(campos, "AtencionMedica", esDato("AtencionMedica", reporte.AtencionMedica));
        AgregarSecundario(campos, "OtrasPersonasAfectadas", esDato("SiNoNoSabe", reporte.OtrasPersonasAfectadas));
        AgregarSecundario(campos, "AfectacionMovilidad", esDato("AfectacionMovilidad", reporte.AfectacionMovilidad));
        AgregarSecundario(campos, "DanosMateriales", esDato("SiNoNoSabe", reporte.DanosMateriales));
        AgregarSecundario(campos, "PerdidaAccesoCuentas", esDato("PerdidaAccesoCuentas", reporte.PerdidaAccesoCuentas));

        // Campos que solo aparecen si la respuesta anterior lo pide
        if (reporte.UsoArma == "SI")
        {
            AgregarSecundario(campos, "TipoArma", esDato("TipoArma", reporte.TipoArma));
        }
        if (reporte.DanosMateriales == "SI")
        {
            AgregarSecundario(campos, "TipoDanoMaterial", esDato("DanoMaterial", reporte.TipoDanoMaterial));
        }

        // Campos de cada bien sustraído
        foreach (var bien in reporte.Bienes)
        {
            AgregarCamposDeBien(campos, bien, esDato);
        }

        return campos;
    }

    // La hora cuenta cuando es exacta; la franja cuenta cuando la precisión es franja aproximada.
    private static bool TieneHoraOFranja(Reporte reporte, Func<string, string?, bool> esDato) => reporte.PrecisionTemporal switch
    {
        "HORA_EXACTA" => reporte.HoraIncidente is not null,
        "FRANJA_APROXIMADA" => esDato("FranjaHoraria", reporte.FranjaHoraria),
        _ => false,
    };

    // Un bien está completo cuando tiene categoría, cantidad y valor con dato.
    private static bool BienCompleto(ReporteBien bien, Func<string, string?, bool> esDato) =>
        esDato("CategoriaBien", bien.CategoriaBien) &&
        esDato("CantidadBien", bien.Cantidad) &&
        esDato("ValorBien", bien.ValorAproximado);

    // Agrega los campos generales del bien y los propios de su categoría.
    private static void AgregarCamposDeBien(List<CampoCompletitud> campos, ReporteBien bien, Func<string, string?, bool> esDato)
    {
        AgregarSecundario(campos, "BienRecuperado", esDato("RecuperadoBien", bien.Recuperado));
        AgregarSecundario(campos, "BienInformacionPersonal", esDato("InformacionPersonalBien", bien.InformacionPersonal));
        AgregarSecundario(campos, "BienBloqueoReportado", esDato("BloqueoReportadoBien", bien.BloqueoReportado));

        if (bien.CategoriaBien == "CELULAR" && bien.Celular is not null)
        {
            AgregarSecundario(campos, "CelularTipo", esDato("TipoDispositivo", bien.Celular.TipoDispositivo));
            AgregarSecundario(campos, "CelularSistemaOperativo", esDato("SistemaOperativo", bien.Celular.SistemaOperativo));
            AgregarSecundario(campos, "CelularEstadoBloqueo", esDato("EstadoBloqueoCelular", bien.Celular.EstadoBloqueo));
            AgregarSecundario(campos, "CelularAppsBancarias", esDato("UsoAppsBancarias", bien.Celular.UsoAppsBancarias));
        }

        if (CategoriasVehiculo.Contains(bien.CategoriaBien) && bien.Vehiculo is not null)
        {
            AgregarSecundario(campos, "VehiculoTipo", esDato("TipoVehiculoBien", bien.Vehiculo.TipoVehiculo));
            AgregarSecundario(campos, "VehiculoEstado", esDato("EstadoVehiculoBien", bien.Vehiculo.Estado));
            AgregarSecundario(campos, "VehiculoModalidad", esDato("ModalidadVehiculoBien", bien.Vehiculo.Modalidad));
            AgregarSecundario(campos, "VehiculoRecuperacion", esDato("RecuperacionVehiculo", bien.Vehiculo.Recuperacion));
        }

        if (bien.CategoriaBien == "TARJETA_BANCARIA" && bien.Tarjeta is not null)
        {
            AgregarSecundario(campos, "TarjetaTipo", esDato("TipoTarjeta", bien.Tarjeta.Tipo));
            AgregarSecundario(campos, "TarjetaEstado", esDato("EstadoTarjeta", bien.Tarjeta.Estado));
            AgregarSecundario(campos, "TarjetaTransacciones", esDato("TransaccionesNoReconocidas", bien.Tarjeta.TransaccionesNoReconocidas));
        }

        if (bien.CategoriaBien == "DOCUMENTOS_PERSONALES" && bien.Documento is not null)
        {
            AgregarSecundario(campos, "DocumentoTipo", esDato("TipoDocumento", bien.Documento.Tipo));
            AgregarSecundario(campos, "DocumentoEstado", esDato("EstadoDocumento", bien.Documento.Estado));
        }

        if (bien.CategoriaBien == "BOLSO_MORRAL_CARTERA")
        {
            AgregarSecundario(campos, "BolsoContenido", bien.ContenidoBolso.Any(c => esDato("ContenidoBolso", c.Codigo)));
        }
    }

    private static void AgregarSecundario(List<CampoCompletitud> campos, string nombre, bool tieneDato) =>
        campos.Add(new CampoCompletitud(nombre, false, tieneDato));
}
