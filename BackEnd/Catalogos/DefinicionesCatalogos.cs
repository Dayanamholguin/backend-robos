namespace AppRobos.Backend.Catalogos;

/// <summary>
/// Una opción de una lista desplegable.
/// CuentaComoDato es FALSE para las respuestas que no suman puntaje en el ICR.
/// </summary>
public sealed record OpcionCatalogo(string Codigo, string Etiqueta, bool CuentaComoDato = true);

/// <summary>
/// Listas desplegables del formulario del reporte (secciones A a H).
/// Se cargan en la base de datos con la migración, en la tabla CatalogoOpciones.
/// Los nombres de catálogo y de código coinciden con los que envía la app.
/// </summary>
public static class DefinicionesCatalogos
{
    // Respuestas que no suman puntaje, reutilizadas en varios catálogos
    private const bool NoSuma = false;

    /// <summary>Catálogos y sus opciones, en el orden en que se muestran.</summary>
    public static readonly IReadOnlyDictionary<string, IReadOnlyList<OpcionCatalogo>> Todos = new Dictionary<string, IReadOnlyList<OpcionCatalogo>>
    {
        // Sección A: fecha, hora y ubicación
        ["PrecisionTemporal"] =
        [
            new("HORA_EXACTA", "Hora exacta"),
            new("FRANJA_APROXIMADA", "Franja aproximada"),
            new("MOMENTO_DESCONOCIDO", "Momento desconocido", NoSuma),
        ],
        ["FranjaHoraria"] =
        [
            new("MADRUGADA", "Madrugada (00:00 a 05:59)"),
            new("MANANA", "Mañana (06:00 a 11:59)"),
            new("TARDE", "Tarde (12:00 a 17:59)"),
            new("NOCHE", "Noche (18:00 a 23:59)"),
        ],
        ["CondicionClima"] =
        [
            new("SOLEADO", "Soleado"),
            new("PARCIALMENTE_NUBLADO", "Parcialmente nublado"),
            new("NUBLADO", "Nublado"),
            new("LLUVIA_LIGERA", "Lluvia ligera"),
            new("LLUVIA_FUERTE", "Lluvia fuerte"),
            new("TORMENTA_ELECTRICA", "Tormenta eléctrica"),
            new("NEBLINA", "Neblina"),
            new("OTRO", "Otro", NoSuma),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["IntensidadLluvia"] =
        [
            new("SIN_LLUVIA", "Sin lluvia"),
            new("LIGERA", "Ligera"),
            new("MODERADA", "Moderada"),
            new("FUERTE", "Fuerte"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["VisibilidadEntorno"] =
        [
            new("BUENA", "Buena"),
            new("REDUCIDA", "Reducida"),
            new("MUY_REDUCIDA", "Muy reducida"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["IluminacionLugar"] =
        [
            new("LUZ_NATURAL", "Luz natural"),
            new("ALUMBRADO_ADECUADO", "Alumbrado público adecuado"),
            new("ALUMBRADO_DEFICIENTE", "Alumbrado deficiente"),
            new("SIN_ILUMINACION", "Sin iluminación"),
            new("NO_OBSERVADA", "Iluminación no observada", NoSuma),
        ],
        ["SiNoNoSabe"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["AfluenciaPersonas"] =
        [
            new("BAJA", "Baja"),
            new("MEDIA", "Media"),
            new("ALTA", "Alta"),
            new("DESCONOCIDA", "Desconocida", NoSuma),
        ],
        ["TipoUbicacion"] =
        [
            new("GPS_ACTUAL", "GPS actual"),
            new("PUNTO_MAPA", "Seleccionar punto en mapa"),
            new("UBICACION_APROXIMADA", "Ubicación aproximada"),
            new("UBICACION_PREVIA", "Ubicación ingresada previamente"),
        ],
        ["TipoLugar"] =
        [
            new("VIA_PUBLICA", "Vía pública"),
            new("ANDEN", "Andén"),
            new("PARQUE", "Parque"),
            new("ESTACION_TRANSPORTE", "Estación de transporte"),
            new("VEHICULO_TRANSPORTE_PUBLICO", "Vehículo o transporte público"),
            new("VIVIENDA", "Vivienda"),
            new("UNIDAD_RESIDENCIAL", "Unidad residencial"),
            new("ESTABLECIMIENTO_COMERCIAL", "Establecimiento comercial"),
            new("OFICINA", "Oficina"),
            new("INSTITUCION_EDUCATIVA", "Institución educativa"),
            new("INSTITUCION_SALUD", "Institución de salud"),
            new("CAJERO_ENTIDAD_FINANCIERA", "Cajero o entidad financiera"),
            new("CENTRO_COMERCIAL", "Centro comercial"),
            new("ESPACIO_DEPORTIVO", "Espacio deportivo"),
            new("OTRO_LUGAR_CATEGORIZADO", "Otro lugar categorizado", NoSuma),
        ],
        ["EntornoLugar"] =
        [
            new("URBANO", "Urbano"),
            new("RURAL", "Rural"),
            new("NO_IDENTIFICADO", "No identificado", NoSuma),
        ],
        ["CaracteristicaSector"] =
        [
            new("ZONA_RESIDENCIAL", "Zona residencial"),
            new("COMERCIAL", "Comercial"),
            new("INDUSTRIAL", "Industrial"),
            new("EDUCATIVA", "Educativa"),
            new("RECREATIVA", "Recreativa"),
            new("FINANCIERA", "Financiera"),
            new("MIXTA", "Mixta"),
            new("NO_IDENTIFICADO", "No identificado", NoSuma),
        ],

        // Sección B: tipo y estado del incidente
        ["TipoIncidente"] =
        [
            new("ROBO_CONSUMADO", "Robo consumado"),
            new("INTENTO_SIN_SUSTRACCION", "Intento de robo sin sustracción"),
            new("INTENTO_SUSTRACCION_PARCIAL", "Intento con sustracción parcial"),
            new("NO_CLASIFICADO", "No está seguro de cómo clasificarlo", NoSuma),
        ],
        ["CategoriaPrincipal"] =
        [
            new("HURTO_PERSONA", "Hurto a persona"),
            new("HURTO_VIVIENDA", "Hurto a vivienda"),
            new("HURTO_ESTABLECIMIENTO_COMERCIAL", "Hurto a establecimiento comercial"),
            new("HURTO_VEHICULO", "Hurto de vehículo"),
            new("HURTO_PARTES_VEHICULO", "Hurto de partes o accesorios de vehículo"),
            new("HURTO_BICICLETA_MOVILIDAD", "Hurto de bicicleta o vehículo de movilidad personal"),
            new("HURTO_TRANSPORTE_PUBLICO", "Hurto en transporte público"),
            new("HURTO_ESPACIO_PUBLICO", "Hurto en infraestructura o espacio público"),
            new("OTRA_CATEGORIA", "Otra categoría", NoSuma),
        ],
        ["ResultadoHecho"] =
        [
            new("SE_LLEVARON_BIENES", "Se llevaron bienes"),
            new("INTENTARON_NO_LOGRARON", "Intentaron llevarse bienes pero no lo lograron"),
            new("RECUPERARON_TODOS", "Recuperaron todos los bienes"),
            new("RECUPERARON_PARTE", "Recuperaron parte de los bienes"),
            new("RESULTADO_DESCONOCIDO", "Resultado desconocido", NoSuma),
        ],
        ["ContextoIncidente"] =
        [
            new("CAMINANDO", "Caminando"),
            new("DESPLAZANDOSE_VEHICULO", "Desplazándose en vehículo"),
            new("TRANSPORTE_PUBLICO", "Usando transporte público"),
            new("DENTRO_VIVIENDA", "Dentro de una vivienda"),
            new("DENTRO_ESTABLECIMIENTO", "Dentro de un establecimiento"),
            new("ESPACIO_PUBLICO", "En un espacio público"),
            new("OTRO_CONTEXTO", "En otro contexto", NoSuma),
        ],
        ["RelacionLugar"] =
        [
            new("VICTIMA_RESIDENTE", "Víctima residente"),
            new("VISITANTE", "Visitante"),
            new("TRABAJADOR", "Trabajador"),
            new("CLIENTE", "Cliente"),
            new("TRANSEUNTE", "Transeúnte"),
            new("NO_APLICA", "No aplica", NoSuma),
            new("NO_SABE", "No sabe", NoSuma),
        ],

        // Sección C: modalidad y circunstancias
        ["ModalidadPercibida"] =
        [
            new("ARREBATO", "Arrebato"),
            new("RAPONAZO", "Raponazo"),
            new("COSQUILLEO_SUSTRACCION", "Cosquilleo o sustracción sin advertencia"),
            new("INTIMIDACION", "Intimidación"),
            new("AMENAZA_CON_ARMA", "Amenaza con arma"),
            new("FUERZA_FISICA", "Fuerza física"),
            new("INGRESO_NO_AUTORIZADO_INMUEBLE", "Ingreso no autorizado a inmueble"),
            new("FORZAMIENTO_CERRADURA_ACCESO", "Forzamiento de cerradura o acceso"),
            new("ENGANO_DISTRACCION", "Engaño o distracción"),
            new("SUSTRACCION_OBJETO_DESATENDIDO", "Sustracción de objeto desatendido"),
            new("ACCESO_NO_AUTORIZADO_VEHICULO", "Acceso no autorizado a vehículo"),
            new("NO_IDENTIFICADA", "Modalidad no identificada", NoSuma),
        ],
        ["TipoArma"] =
        [
            new("ARMA_FUEGO", "Arma de fuego"),
            new("ARMA_CORTOPUNZANTE", "Arma cortopunzante"),
            new("OBJETO_CONTUNDENTE", "Objeto contundente"),
            new("OBJETO_INTIMIDAR", "Objeto usado para intimidar"),
            new("ARMA_NO_IDENTIFICADA", "Arma no identificada", NoSuma),
        ],
        ["IngresoInmueble"] =
        [
            new("CON_AUTORIZACION_INICIAL", "Sí, con autorización inicial"),
            new("SIN_AUTORIZACION", "Sí, sin autorización"),
            new("NO", "No"),
            new("NO_APLICA", "No aplica", NoSuma),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["AccesoForzado"] =
        [
            new("PUERTA", "Puerta"),
            new("VENTANA", "Ventana"),
            new("CERRADURA", "Cerradura"),
            new("REJA", "Reja"),
            new("ACCESO_VEHICULAR", "Acceso vehicular"),
            new("OTRO_ACCESO", "Otro acceso", NoSuma),
            new("NINGUNO", "Ninguno"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["MedioAproximacion"] =
        [
            new("A_PIE", "A pie"),
            new("MOTOCICLETA", "Motocicleta"),
            new("AUTOMOVIL", "Automóvil"),
            new("BICICLETA", "Bicicleta"),
            new("TRANSPORTE_PUBLICO", "Transporte público"),
            new("OTRO_VEHICULO", "Otro vehículo"),
            new("NO_IDENTIFICADO", "No se identificó", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["MomentoIntervencion"] =
        [
            new("SIN_INTERACCION_PREVIA", "El hecho ocurrió sin interacción previa"),
            new("ACERCAMIENTO_CONVERSACION", "Hubo acercamiento o conversación"),
            new("FORCEJEO", "Hubo forcejeo"),
            new("PERSECUCION", "Hubo persecución"),
            new("NO_SABE", "No sabe", NoSuma),
        ],

        // Sección D: bienes sustraídos
        ["CategoriaBien"] =
        [
            new("CELULAR", "Celular"),
            new("COMPUTADOR_TABLETA", "Computador o tableta"),
            new("RELOJ_ACCESORIO", "Reloj o accesorio"),
            new("BOLSO_MORRAL_CARTERA", "Bolso, morral o cartera"),
            new("DINERO_EFECTIVO", "Dinero en efectivo"),
            new("TARJETA_BANCARIA", "Tarjeta bancaria"),
            new("DOCUMENTOS_PERSONALES", "Documentos personales"),
            new("JOYAS", "Joyas"),
            new("BICICLETA", "Bicicleta"),
            new("MOTOCICLETA", "Motocicleta"),
            new("AUTOMOVIL", "Automóvil"),
            new("PARTES_VEHICULO", "Partes de vehículo"),
            new("ELECTRODOMESTICOS", "Electrodomésticos"),
            new("HERRAMIENTAS", "Herramientas"),
            new("MERCANCIA", "Mercancía"),
            new("ROPA_CALZADO", "Ropa o calzado"),
            new("EQUIPOS_TRABAJO", "Equipos de trabajo"),
            new("OTROS_BIENES", "Otros bienes", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["CantidadBien"] =
        [
            new("1", "1"),
            new("2", "2"),
            new("3", "3"),
            new("4", "4"),
            new("5_O_MAS", "5 o más"),
            new("DESCONOCIDA", "Desconocida", NoSuma),
        ],
        ["ValorBien"] =
        [
            new("SIN_VALOR_ESTIMADO", "Sin valor estimado"),
            new("MENOS_100000", "Menos de $100.000"),
            new("100000_499999", "$100.000 a $499.999"),
            new("500000_999999", "$500.000 a $999.999"),
            new("1000000_4999999", "$1.000.000 a $4.999.999"),
            new("5000000_9999999", "$5.000.000 a $9.999.999"),
            new("10000000_O_MAS", "$10.000.000 o más"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["RecuperadoBien"] =
        [
            new("SI_TOTAL", "Sí, totalmente"),
            new("SI_PARCIAL", "Sí, parcialmente"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["InformacionPersonalBien"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["BloqueoReportadoBien"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("EN_PROCESO", "En proceso"),
            new("NO_SABE", "No sabe", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["TipoDispositivo"] =
        [
            new("TELEFONO_INTELIGENTE", "Teléfono inteligente"),
            new("TELEFONO_BASICO", "Teléfono básico"),
            new("OTRO_DISPOSITIVO", "Otro dispositivo móvil", NoSuma),
        ],
        ["SistemaOperativo"] =
        [
            new("ANDROID", "Android"),
            new("IOS", "iOS"),
            new("OTRO", "Otro", NoSuma),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["EstadoBloqueoCelular"] =
        [
            new("BLOQUEADO", "Bloqueado"),
            new("PENDIENTE_BLOQUEO", "Pendiente de bloqueo"),
            new("NO_BLOQUEADO", "No bloqueado"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["UsoAppsBancarias"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("PREFIERE_NO_RESPONDER", "Prefiere no responder", NoSuma),
        ],
        ["TipoVehiculoBien"] =
        [
            new("AUTOMOVIL", "Automóvil"),
            new("MOTOCICLETA", "Motocicleta"),
            new("BICICLETA", "Bicicleta"),
            new("OTRO_VEHICULO", "Otro vehículo"),
        ],
        ["EstadoVehiculoBien"] =
        [
            new("ESTACIONADO", "Estacionado"),
            new("EN_CIRCULACION", "En circulación"),
            new("OTRO", "Otro", NoSuma),
        ],
        ["ModalidadVehiculoBien"] =
        [
            new("ROBO_COMPLETO", "Robo completo"),
            new("INTENTO_ROBO", "Intento de robo"),
            new("SUSTRACCION_ACCESORIOS", "Sustracción de accesorios"),
        ],
        ["RecuperacionVehiculo"] =
        [
            new("RECUPERADO", "Recuperado"),
            new("NO_RECUPERADO", "No recuperado"),
            new("PARCIALMENTE_RECUPERADO", "Parcialmente recuperado"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["TipoTarjeta"] =
        [
            new("DEBITO", "Débito"),
            new("CREDITO", "Crédito"),
            new("PREPAGO", "Prepago"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["EstadoTarjeta"] =
        [
            new("BLOQUEADA", "Bloqueada"),
            new("PENDIENTE_BLOQUEO", "Pendiente de bloqueo"),
            new("NO_BLOQUEADA", "No bloqueada"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["TransaccionesNoReconocidas"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("PENDIENTE_VERIFICAR", "Pendiente de verificar", NoSuma),
        ],
        ["TipoDocumento"] =
        [
            new("DOCUMENTO_IDENTIDAD", "Documento de identidad"),
            new("LICENCIA_CONDUCCION", "Licencia de conducción"),
            new("PASAPORTE", "Pasaporte"),
            new("DOCUMENTOS_BANCARIOS", "Documentos bancarios"),
            new("OTROS_DOCUMENTOS", "Otros documentos", NoSuma),
        ],
        ["EstadoDocumento"] =
        [
            new("PERDIDO_O_ROBADO", "Reportados como perdidos o robados"),
            new("EN_REPOSICION", "En proceso de reposición"),
            new("REPUESTO", "Repuestos"),
            new("PENDIENTE", "Pendiente", NoSuma),
        ],
        ["ContenidoBolso"] =
        [
            new("DOCUMENTOS", "Documentos"),
            new("DINERO", "Dinero"),
            new("TARJETAS", "Tarjetas"),
            new("CELULAR", "Celular"),
            new("LLAVES", "Llaves"),
            new("OBJETOS_PERSONALES", "Objetos personales"),
            new("CONTENIDO_DESCONOCIDO", "Contenido desconocido", NoSuma),
        ],

        // Sección E: presuntos responsables (opcional)
        ["CantidadResponsables"] =
        [
            new("1", "1"),
            new("2", "2"),
            new("3", "3"),
            new("4_O_MAS", "4 o más"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["MedioHuida"] =
        [
            new("A_PIE", "A pie"),
            new("MOTOCICLETA", "Motocicleta"),
            new("AUTOMOVIL", "Automóvil"),
            new("BICICLETA", "Bicicleta"),
            new("TRANSPORTE_PUBLICO", "Transporte público"),
            new("OTRO", "Otro", NoSuma),
            new("NO_IDENTIFICADO", "No identificado", NoSuma),
            new("NO_HUBO_HUIDA_OBSERVADA", "No hubo huida observada"),
        ],
        ["RostroObservado"] =
        [
            new("SI", "Sí"),
            new("PARCIALMENTE", "Parcialmente"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["VestimentaObservada"] =
        [
            new("SI", "Sí"),
            new("PARCIALMENTE", "Parcialmente"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["TipoVehiculoObservado"] =
        [
            new("MOTOCICLETA", "Motocicleta"),
            new("AUTOMOVIL", "Automóvil"),
            new("CAMIONETA", "Camioneta"),
            new("BICICLETA", "Bicicleta"),
            new("PATINETA_ELECTRICA", "Patineta eléctrica"),
            new("OTRO", "Otro", NoSuma),
            new("NO_IDENTIFICADO", "No identificado", NoSuma),
        ],
        ["PlacaObservada"] =
        [
            new("COMPLETA", "Completa"),
            new("PARCIAL", "Parcial"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
        ],
        ["CaracteristicaDistintiva"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("NO_RECUERDA", "No recuerda", NoSuma),
        ],

        // Sección F: consecuencias y afectación
        ["LesionesFisicas"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
            new("PREFIERE_NO_RESPONDER", "Prefiere no responder", NoSuma),
        ],
        ["GravedadPercibida"] =
        [
            new("SIN_LESIONES", "Sin lesiones"),
            new("LEVES", "Leves"),
            new("REQUIRIERON_ATENCION_MEDICA", "Requirieron atención médica"),
            new("GRAVES", "Graves"),
            new("NO_SABE", "No sabe", NoSuma),
            new("PREFIERE_NO_RESPONDER", "Prefiere no responder", NoSuma),
        ],
        ["AtencionMedica"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("PENDIENTE", "Pendiente"),
            new("PREFIERE_NO_RESPONDER", "Prefiere no responder", NoSuma),
        ],
        ["AfectacionMovilidad"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["DanoMaterial"] =
        [
            new("PUERTA", "Puerta"),
            new("VENTANA", "Ventana"),
            new("CERRADURA", "Cerradura"),
            new("VEHICULO", "Vehículo"),
            new("MOBILIARIO", "Mobiliario"),
            new("INFRAESTRUCTURA", "Infraestructura"),
            new("OTRO", "Otro", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["PerdidaAccesoCuentas"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],

        // Sección G: denuncia y actuaciones posteriores
        ["InformoAutoridad"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("EN_PROCESO", "En proceso"),
            new("NO_SABE", "No sabe", NoSuma),
            new("PREFIERE_NO_RESPONDER", "Prefiere no responder", NoSuma),
        ],
        ["AutoridadCanal"] =
        [
            new("POLICIA", "Policía"),
            new("FISCALIA", "Fiscalía"),
            new("LINEA_EMERGENCIA", "Línea de emergencia"),
            new("ADMINISTRACION_LUGAR", "Administración del lugar"),
            new("EMPRESA_SEGURIDAD", "Empresa de seguridad"),
            new("OTRO_CANAL", "Otro canal", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["EstadoDenunciaFormal"] =
        [
            new("PRESENTADA", "Presentada"),
            new("PENDIENTE_PRESENTACION", "Pendiente de presentación"),
            new("NO_PRESENTADA", "No presentada"),
            new("NO_SABE", "No sabe", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["TieneRadicado"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["NotificoBanco"] =
        [
            new("SI", "Sí"),
            new("NO", "No"),
            new("EN_PROCESO", "En proceso"),
            new("NO_APLICA", "No aplica", NoSuma),
        ],
        ["RecuperacionActuacion"] =
        [
            new("SI_TODOS", "Sí, todos"),
            new("SI_ALGUNOS", "Sí, algunos"),
            new("NO", "No"),
            new("NO_SABE", "No sabe", NoSuma),
            new("NO_APLICA", "No aplica", NoSuma),
        ],

        // Sección H: datos de quien reporta
        ["TipoParticipacion"] =
        [
            new("VICTIMA_DIRECTA", "Víctima directa"),
            new("TESTIGO", "Testigo"),
            new("FAMILIAR_ACOMPANANTE", "Familiar o acompañante"),
            new("OTRA_PERSONA", "Otra persona que reporta"),
        ],
    };
}
