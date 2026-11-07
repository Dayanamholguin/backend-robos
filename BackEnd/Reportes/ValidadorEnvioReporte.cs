namespace AppRobos.Backend.Reportes;

// Revisa que un reporte tenga todos los campos obligatorios antes de enviarlo.
// Una respuesta como "no sabe" o "prefiere no responder" cuenta como respondida,
// aunque no sume puntaje en la completitud.
public static class ValidadorEnvioReporte
{
    // Devuelve los mensajes de error en español. Lista vacía si el reporte se puede enviar.
    public static IReadOnlyList<string> Validar(Reporte reporte, bool cedulaRegistrada)
    {
        var errores = new List<string>();

        ValidarSeccionA(reporte, errores);
        ValidarSeccionB(reporte, errores);
        ValidarSeccionC(reporte, errores);
        ValidarSeccionD(reporte, errores);
        ValidarSeccionF(reporte, errores);
        ValidarSeccionH(reporte, cedulaRegistrada, errores);

        return errores;
    }

    private static void ValidarSeccionA(Reporte r, List<string> errores)
    {
        if (r.FechaIncidente is null) errores.Add("Falta la fecha del incidente.");
        if (r.PrecisionTemporal is null) errores.Add("Falta la precisión temporal del incidente.");
        if (r.PrecisionTemporal == "HORA_EXACTA" && r.HoraIncidente is null) errores.Add("Falta la hora del incidente.");
        if (r.PrecisionTemporal == "FRANJA_APROXIMADA" && r.FranjaHoraria is null) errores.Add("Falta la franja horaria del incidente.");
        if (r.CondicionClima is null) errores.Add("Falta la condición climática.");
        if (r.IntensidadLluvia is null) errores.Add("Falta la intensidad de la lluvia.");
        if (r.VisibilidadEntorno is null) errores.Add("Falta la visibilidad del entorno.");
        if (r.IluminacionLugar is null) errores.Add("Falta la iluminación del lugar.");
        if (r.SuperficieMojada is null) errores.Add("Falta indicar si la superficie está mojada.");
        if (r.AfluenciaPersonas is null) errores.Add("Falta la afluencia de personas.");
        if (r.MunicipioCodigo is null) errores.Add("Falta el municipio.");
        if (r.BarrioId is null) errores.Add("Falta el barrio o sector.");
        if (r.TipoUbicacion is null) errores.Add("Falta el tipo de ubicación.");
        if (r.Latitud is null || r.Longitud is null) errores.Add("Falta la ubicación del incidente.");
        if (r.TipoLugar is null) errores.Add("Falta el tipo de lugar.");
        if (r.EntornoLugar is null) errores.Add("Falta el entorno del lugar.");
    }

    private static void ValidarSeccionB(Reporte r, List<string> errores)
    {
        if (r.TipoIncidente is null) errores.Add("Falta el tipo de incidente.");
        if (r.CategoriaPrincipal is null) errores.Add("Falta la categoría principal.");
        if (r.ResultadoHecho is null) errores.Add("Falta el resultado del hecho.");
        if (r.ContextoIncidente is null) errores.Add("Falta el contexto del incidente.");
        if (r.RelacionLugar is null) errores.Add("Falta la relación con el lugar.");
    }

    private static void ValidarSeccionC(Reporte r, List<string> errores)
    {
        if (r.ModalidadPercibida is null) errores.Add("Falta la modalidad percibida.");
        if (r.ContactoFisico is null) errores.Add("Falta indicar si hubo contacto físico.");
        if (r.Amenazas is null) errores.Add("Falta indicar si hubo amenazas.");
        if (r.UsoArma is null) errores.Add("Falta indicar si se usó un arma.");
        if (r.UsoArma == "SI" && r.TipoArma is null) errores.Add("Falta el tipo de arma.");
        if (r.IngresoInmueble is null) errores.Add("Falta indicar si hubo ingreso a un inmueble.");
        if (r.AccesosForzados.Count == 0) errores.Add("Falta indicar si se forzó algún acceso.");
        if (r.SeguimientoPrevio is null) errores.Add("Falta indicar si la víctima fue seguida.");
        if (r.DistraccionPrevia is null) errores.Add("Falta indicar si hubo distracción previa.");
        if (r.UsoVehiculo is null) errores.Add("Falta indicar si se usó un vehículo.");
        if (r.MedioAproximacion is null) errores.Add("Falta el medio de aproximación o huida.");
        if (r.MomentoIntervencion is null) errores.Add("Falta el momento de la intervención.");
    }

    private static void ValidarSeccionD(Reporte r, List<string> errores)
    {
        if (r.Bienes.Count == 0)
        {
            errores.Add("Debes registrar al menos un bien sustraído.");
            return;
        }

        foreach (var bien in r.Bienes)
        {
            if (bien.CategoriaBien.Length == 0) errores.Add("Falta la categoría de un bien.");
            if (bien.Cantidad.Length == 0) errores.Add("Falta la cantidad de un bien.");
            if (bien.ValorAproximado.Length == 0) errores.Add("Falta el valor aproximado de un bien.");
            if (bien.Recuperado is null) errores.Add("Falta indicar si se recuperó un bien.");
            if (bien.InformacionPersonal is null) errores.Add("Falta indicar si el bien tenía información personal.");
            if (bien.BloqueoReportado is null) errores.Add("Falta indicar si se bloqueó o reportó un bien.");

            ValidarCamposDeCategoria(bien, errores);
        }
    }

    // Campos propios de cada categoría de bien.
    private static void ValidarCamposDeCategoria(ReporteBien bien, List<string> errores)
    {
        if (bien.CategoriaBien == "CELULAR")
        {
            if (bien.Celular is null || bien.Celular.TipoDispositivo is null || bien.Celular.SistemaOperativo is null ||
                bien.Celular.EstadoBloqueo is null || bien.Celular.UsoAppsBancarias is null)
            {
                errores.Add("Faltan datos del celular sustraído.");
            }
        }

        if (bien.CategoriaBien is "AUTOMOVIL" or "MOTOCICLETA" or "BICICLETA")
        {
            if (bien.Vehiculo is null || bien.Vehiculo.TipoVehiculo is null || bien.Vehiculo.Estado is null ||
                bien.Vehiculo.Modalidad is null || bien.Vehiculo.Recuperacion is null)
            {
                errores.Add("Faltan datos del vehículo sustraído.");
            }
        }

        if (bien.CategoriaBien == "TARJETA_BANCARIA")
        {
            if (bien.Tarjeta is null || bien.Tarjeta.Tipo is null || bien.Tarjeta.Estado is null || bien.Tarjeta.TransaccionesNoReconocidas is null)
            {
                errores.Add("Faltan datos de la tarjeta bancaria sustraída.");
            }
        }

        if (bien.CategoriaBien == "DOCUMENTOS_PERSONALES")
        {
            if (bien.Documento is null || bien.Documento.Tipo is null || bien.Documento.Estado is null)
            {
                errores.Add("Faltan datos de los documentos sustraídos.");
            }
        }

        if (bien.CategoriaBien == "BOLSO_MORRAL_CARTERA" && bien.ContenidoBolso.Count == 0)
        {
            errores.Add("Indica el contenido del bolso o cartera sustraído.");
        }
    }

    private static void ValidarSeccionF(Reporte r, List<string> errores)
    {
        if (r.LesionesFisicas is null) errores.Add("Falta indicar si hubo lesiones físicas.");
        if (r.GravedadPercibida is null) errores.Add("Falta la gravedad percibida.");
        if (r.AtencionMedica is null) errores.Add("Falta indicar si requirió atención médica.");
        if (r.OtrasPersonasAfectadas is null) errores.Add("Falta indicar si hubo otras personas afectadas.");
        if (r.AfectacionMovilidad is null) errores.Add("Falta indicar si se afectó la movilidad.");
        if (r.DanosMateriales is null) errores.Add("Falta indicar si hubo daños materiales.");
        if (r.DanosMateriales == "SI" && r.TipoDanoMaterial is null) errores.Add("Falta el tipo de daño material.");
        if (r.PerdidaAccesoCuentas is null) errores.Add("Falta indicar si se perdió acceso a cuentas o servicios.");
    }

    private static void ValidarSeccionH(Reporte r, bool cedulaRegistrada, List<string> errores)
    {
        var datos = r.DatosPersonales;
        if (datos is null)
        {
            errores.Add("Faltan los datos de quien reporta.");
            return;
        }

        if (datos.TipoParticipacion.Length == 0) errores.Add("Falta el tipo de participación.");
        if (!datos.ConsentimientoAceptado) errores.Add("Debes aceptar el tratamiento de datos para enviar el reporte.");
        if (datos.NombreCompleto.Length == 0) errores.Add("Falta el nombre completo de quien reporta.");
        if (!cedulaRegistrada) errores.Add("Falta la cédula de quien reporta.");
        if (datos.Correo.Length == 0) errores.Add("Falta el correo de quien reporta.");
        if (datos.Celular.Length == 0) errores.Add("Falta el celular de quien reporta.");
    }
}
