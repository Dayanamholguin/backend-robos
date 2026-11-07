using System.Text.RegularExpressions;
using AppRobos.Backend.Catalogos;
using AppRobos.Backend.Dtos;
using AppRobos.Backend.Territorio;

namespace AppRobos.Backend.Reportes;

// Revisa los datos de un guardado parcial: valores de las listas, formatos y ubicación.
// No exige campos obligatorios: eso se revisa al enviar.
public class ValidadorGuardadoReporte
{
    private static readonly Regex Radicado = new(@"^[A-Za-z0-9\-/]{4,40}$", RegexOptions.Compiled);
    private static readonly Regex Cedula = new(@"^\d{6,10}$", RegexOptions.Compiled);
    private static readonly Regex Celular = new(@"^3\d{9}$", RegexOptions.Compiled);
    private static readonly Regex Correo = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private readonly ServicioOpcionesFormulario _opciones;
    private readonly ServicioTerritorio _territorio;
    private readonly TimeProvider _reloj;

    public ValidadorGuardadoReporte(ServicioOpcionesFormulario opciones, ServicioTerritorio territorio, TimeProvider reloj)
    {
        _opciones = opciones;
        _territorio = territorio;
        _reloj = reloj;
    }

    // Devuelve los mensajes de error en español. Lista vacía si el guardado es válido.
    public async Task<IReadOnlyList<string>> ValidarAsync(ReporteEntradaDto dto, CancellationToken ct)
    {
        var errores = new List<string>();
        var catalogos = await _opciones.CargarAsync(ct);

        ValidarOpciones(dto, catalogos, errores);
        ValidarFormatos(dto, errores);
        await ValidarTerritorioAsync(dto, errores, ct);

        return errores;
    }

    // Cada valor de lista debe existir y estar habilitado en su catálogo.
    private static void ValidarOpciones(ReporteEntradaDto dto, OpcionesCargadas catalogos, List<string> errores)
    {
        var valores = new List<(string Campo, string Catalogo, string? Codigo)>
        {
            ("precisión temporal", "PrecisionTemporal", dto.PrecisionTemporal),
            ("condición climática", "CondicionClima", dto.CondicionClima),
            ("intensidad de la lluvia", "IntensidadLluvia", dto.IntensidadLluvia),
            ("visibilidad del entorno", "VisibilidadEntorno", dto.VisibilidadEntorno),
            ("iluminación del lugar", "IluminacionLugar", dto.IluminacionLugar),
            ("superficie mojada", "SiNoNoSabe", dto.SuperficieMojada),
            ("afluencia de personas", "AfluenciaPersonas", dto.AfluenciaPersonas),
            ("tipo de ubicación", "TipoUbicacion", dto.TipoUbicacion),
            ("tipo de lugar", "TipoLugar", dto.TipoLugar),
            ("entorno", "EntornoLugar", dto.EntornoLugar),
            ("tipo de incidente", "TipoIncidente", dto.TipoIncidente),
            ("categoría principal", "CategoriaPrincipal", dto.CategoriaPrincipal),
            ("resultado del hecho", "ResultadoHecho", dto.ResultadoHecho),
            ("contexto del incidente", "ContextoIncidente", dto.ContextoIncidente),
            ("relación con el lugar", "RelacionLugar", dto.RelacionLugar),
            ("modalidad percibida", "ModalidadPercibida", dto.ModalidadPercibida),
            ("contacto físico", "SiNoNoSabe", dto.ContactoFisico),
            ("amenazas", "SiNoNoSabe", dto.Amenazas),
            ("uso de arma", "SiNoNoSabe", dto.UsoArma),
            ("ingreso a inmueble", "IngresoInmueble", dto.IngresoInmueble),
            ("seguimiento previo", "SiNoNoSabe", dto.SeguimientoPrevio),
            ("distracción previa", "SiNoNoSabe", dto.DistraccionPrevia),
            ("uso de vehículo", "SiNoNoSabe", dto.UsoVehiculo),
            ("medio de aproximación", "MedioAproximacion", dto.MedioAproximacion),
            ("momento de la intervención", "MomentoIntervencion", dto.MomentoIntervencion),
            ("lesiones físicas", "LesionesFisicas", dto.LesionesFisicas),
            ("gravedad percibida", "GravedadPercibida", dto.GravedadPercibida),
            ("atención médica", "AtencionMedica", dto.AtencionMedica),
            ("otras personas afectadas", "SiNoNoSabe", dto.OtrasPersonasAfectadas),
            ("afectación a la movilidad", "AfectacionMovilidad", dto.AfectacionMovilidad),
            ("daños materiales", "SiNoNoSabe", dto.DanosMateriales),
            ("pérdida de acceso a cuentas", "PerdidaAccesoCuentas", dto.PerdidaAccesoCuentas),
            ("informó a una autoridad", "InformoAutoridad", dto.InformoAutoridad),
            ("autoridad o canal", "AutoridadCanal", dto.AutoridadCanal),
            ("estado de la denuncia", "EstadoDenunciaFormal", dto.EstadoDenunciaFormal),
            ("tiene radicado", "TieneRadicado", dto.TieneRadicado),
            ("notificó al banco", "NotificoBanco", dto.NotificoBanco),
            ("recuperación por actuación", "RecuperacionActuacion", dto.RecuperacionActuacion),
            ("tipo de participación", "TipoParticipacion", dto.DatosPersonales?.TipoParticipacion),
        };

        if (dto.UsoArma == "SI") valores.Add(("tipo de arma", "TipoArma", dto.TipoArma));
        if (dto.PrecisionTemporal == "FRANJA_APROXIMADA") valores.Add(("franja horaria", "FranjaHoraria", dto.FranjaHoraria));
        if (dto.DanosMateriales == "SI") valores.Add(("tipo de daño material", "DanoMaterial", dto.TipoDanoMaterial));
        if (dto.Responsables is { } resp)
        {
            valores.Add(("cantidad de responsables", "CantidadResponsables", resp.CantidadResponsables));
            valores.Add(("responsables coordinados", "SiNoNoSabe", resp.ActuaronCoordinados));
            valores.Add(("medio de huida", "MedioHuida", resp.MedioHuida));
            valores.Add(("rostro observado", "RostroObservado", resp.RostroObservado));
            valores.Add(("vestimenta observada", "VestimentaObservada", resp.VestimentaObservada));
            valores.Add(("vehículo observado", "SiNoNoSabe", resp.VehiculoObservado));
            valores.Add(("placa observada", "PlacaObservada", resp.PlacaObservada));
            valores.Add(("característica distintiva", "CaracteristicaDistintiva", resp.CaracteristicaDistintiva));
            if (resp.VehiculoObservado == "SI") valores.Add(("tipo de vehículo observado", "TipoVehiculoObservado", resp.TipoVehiculo));
        }

        foreach (var (campo, catalogo, codigo) in valores)
        {
            if (codigo is not null && !catalogos.Existe(catalogo, codigo))
            {
                errores.Add($"El valor de {campo} no es válido.");
            }
        }

        foreach (var caracteristica in dto.CaracteristicasSector ?? [])
        {
            if (!catalogos.Existe("CaracteristicaSector", caracteristica)) errores.Add("Una característica del sector no es válida.");
        }
        foreach (var acceso in dto.AccesosForzados ?? [])
        {
            if (!catalogos.Existe("AccesoForzado", acceso)) errores.Add("Una forma de acceso forzado no es válida.");
        }

        foreach (var bien in dto.Bienes ?? [])
        {
            ValidarBien(bien, catalogos, errores);
        }
    }

    // Valores de los bienes y de sus campos propios.
    private static void ValidarBien(BienEntradaDto bien, OpcionesCargadas catalogos, List<string> errores)
    {
        var valores = new List<(string Catalogo, string? Codigo)>
        {
            ("CategoriaBien", bien.CategoriaBien),
            ("CantidadBien", bien.Cantidad),
            ("ValorBien", bien.ValorAproximado),
            ("RecuperadoBien", bien.Recuperado),
            ("InformacionPersonalBien", bien.InformacionPersonal),
            ("BloqueoReportadoBien", bien.BloqueoReportado),
        };

        if (bien.Celular is { } celular)
        {
            valores.Add(("TipoDispositivo", celular.TipoDispositivo));
            valores.Add(("SistemaOperativo", celular.SistemaOperativo));
            valores.Add(("EstadoBloqueoCelular", celular.EstadoBloqueo));
            valores.Add(("UsoAppsBancarias", celular.UsoAppsBancarias));
        }
        if (bien.Vehiculo is { } vehiculo)
        {
            valores.Add(("TipoVehiculoBien", vehiculo.TipoVehiculo));
            valores.Add(("EstadoVehiculoBien", vehiculo.Estado));
            valores.Add(("ModalidadVehiculoBien", vehiculo.Modalidad));
            valores.Add(("RecuperacionVehiculo", vehiculo.Recuperacion));
        }
        if (bien.Tarjeta is { } tarjeta)
        {
            valores.Add(("TipoTarjeta", tarjeta.Tipo));
            valores.Add(("EstadoTarjeta", tarjeta.Estado));
            valores.Add(("TransaccionesNoReconocidas", tarjeta.TransaccionesNoReconocidas));
        }
        if (bien.Documento is { } documento)
        {
            valores.Add(("TipoDocumento", documento.Tipo));
            valores.Add(("EstadoDocumento", documento.Estado));
        }

        foreach (var (catalogo, codigo) in valores)
        {
            if (codigo is not null && !catalogos.Existe(catalogo, codigo))
            {
                errores.Add($"Un valor de los bienes sustraídos no es válido ({catalogo}).");
            }
        }

        foreach (var contenido in bien.ContenidoBolso ?? [])
        {
            if (!catalogos.Existe("ContenidoBolso", contenido)) errores.Add("Un contenido del bolso no es válido.");
        }
    }

    // Formatos de los campos de texto que sí se capturan.
    private static void ValidarFormatos(ReporteEntradaDto dto, List<string> errores)
    {
        if (dto.NumeroRadicado is { } radicado && radicado.Trim().Length > 0 && !Radicado.IsMatch(radicado.Trim()))
        {
            errores.Add("El número de radicado no tiene un formato válido.");
        }

        if (dto.DatosPersonales is { } datos)
        {
            if (!string.IsNullOrWhiteSpace(datos.Cedula) && !Cedula.IsMatch(datos.Cedula.Trim()))
            {
                errores.Add("La cédula debe tener entre 6 y 10 dígitos.");
            }
            if (!string.IsNullOrWhiteSpace(datos.Celular) && !Celular.IsMatch(datos.Celular.Trim()))
            {
                errores.Add("El celular debe tener 10 dígitos y empezar por 3 (celular de Colombia).");
            }
            if (!string.IsNullOrWhiteSpace(datos.Correo) && !Correo.IsMatch(datos.Correo.Trim()))
            {
                errores.Add("El correo electrónico no tiene un formato válido.");
            }
        }
    }

    // Fecha, coordenadas, municipio y barrio deben ser coherentes entre sí.
    private async Task ValidarTerritorioAsync(ReporteEntradaDto dto, List<string> errores, CancellationToken ct)
    {
        var hoyLocal = DateOnly.FromDateTime(_reloj.GetUtcNow().ToOffset(MapeadorReporte.DesfaseLocal).DateTime);
        if (dto.FechaIncidente is { } fecha && fecha > hoyLocal)
        {
            errores.Add("La fecha del incidente no puede ser posterior a hoy.");
        }

        var tieneCoordenadas = dto.Latitud is not null && dto.Longitud is not null;
        if (dto.Latitud is { } lat && (lat < -90 || lat > 90))
        {
            errores.Add("La latitud no es válida.");
        }
        if (dto.Longitud is { } lon && (lon < -180 || lon > 180))
        {
            errores.Add("La longitud no es válida.");
        }

        if (tieneCoordenadas && errores.Count == 0)
        {
            var enCobertura = await _territorio.UbicacionEnCoberturaAsync(dto.Latitud!.Value, dto.Longitud!.Value, ct);
            if (!enCobertura)
            {
                errores.Add("La ubicación está fuera del área metropolitana. La app no está habilitada para esta zona.");
            }
        }

        if (dto.BarrioId is { } barrioId && dto.MunicipioCodigo is { } municipio)
        {
            if (!await _territorio.BarrioPerteneceAMunicipioAsync(barrioId, municipio, ct))
            {
                errores.Add("El barrio o sector no corresponde al municipio seleccionado.");
            }
            else if (tieneCoordenadas && errores.Count == 0 &&
                     !await _territorio.BarrioContieneUbicacionAsync(barrioId, dto.Latitud!.Value, dto.Longitud!.Value, ct))
            {
                errores.Add("La ubicación no corresponde al barrio o sector seleccionado.");
            }
        }
    }
}
