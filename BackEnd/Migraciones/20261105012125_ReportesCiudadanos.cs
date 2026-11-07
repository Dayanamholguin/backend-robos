using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppRobos.Backend.Migraciones
{
    /// <inheritdoc />
    public partial class ReportesCiudadanos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoberturaMetropolitana",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoberturaMetropolitana", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CriteriosIcr",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Peso = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriosIcr", x => x.Codigo);
                    table.CheckConstraint("CK_CriteriosIcr_Peso", "\"Peso\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "TerritorioComunas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerritorioComunas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerritorioMunicipios",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerritorioMunicipios", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TerritorioBarrios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    MunicipioCodigo = table.Column<string>(type: "text", nullable: false),
                    ComunaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerritorioBarrios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerritorioBarrios_TerritorioComunas_ComunaId",
                        column: x => x.ComunaId,
                        principalTable: "TerritorioComunas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TerritorioBarrios_TerritorioMunicipios_MunicipioCodigo",
                        column: x => x.MunicipioCodigo,
                        principalTable: "TerritorioMunicipios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reporte",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstadoReporte = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    FechaPrimerGuardado = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaVencimientoBorrador = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FechaEnvio = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FechaActualizacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaIncidente = table.Column<DateOnly>(type: "date", nullable: true),
                    HoraIncidente = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    PrecisionTemporal = table.Column<string>(type: "text", nullable: true),
                    FranjaHoraria = table.Column<string>(type: "text", nullable: true),
                    CondicionClima = table.Column<string>(type: "text", nullable: true),
                    IntensidadLluvia = table.Column<string>(type: "text", nullable: true),
                    VisibilidadEntorno = table.Column<string>(type: "text", nullable: true),
                    IluminacionLugar = table.Column<string>(type: "text", nullable: true),
                    SuperficieMojada = table.Column<string>(type: "text", nullable: true),
                    AfluenciaPersonas = table.Column<string>(type: "text", nullable: true),
                    MunicipioCodigo = table.Column<string>(type: "text", nullable: true),
                    BarrioId = table.Column<int>(type: "integer", nullable: true),
                    TipoUbicacion = table.Column<string>(type: "text", nullable: true),
                    Latitud = table.Column<double>(type: "double precision", nullable: true),
                    Longitud = table.Column<double>(type: "double precision", nullable: true),
                    TipoLugar = table.Column<string>(type: "text", nullable: true),
                    EntornoLugar = table.Column<string>(type: "text", nullable: true),
                    TipoIncidente = table.Column<string>(type: "text", nullable: true),
                    CategoriaPrincipal = table.Column<string>(type: "text", nullable: true),
                    ResultadoHecho = table.Column<string>(type: "text", nullable: true),
                    ContextoIncidente = table.Column<string>(type: "text", nullable: true),
                    RelacionLugar = table.Column<string>(type: "text", nullable: true),
                    ModalidadPercibida = table.Column<string>(type: "text", nullable: true),
                    ContactoFisico = table.Column<string>(type: "text", nullable: true),
                    Amenazas = table.Column<string>(type: "text", nullable: true),
                    UsoArma = table.Column<string>(type: "text", nullable: true),
                    TipoArma = table.Column<string>(type: "text", nullable: true),
                    IngresoInmueble = table.Column<string>(type: "text", nullable: true),
                    SeguimientoPrevio = table.Column<string>(type: "text", nullable: true),
                    DistraccionPrevia = table.Column<string>(type: "text", nullable: true),
                    UsoVehiculo = table.Column<string>(type: "text", nullable: true),
                    MedioAproximacion = table.Column<string>(type: "text", nullable: true),
                    MomentoIntervencion = table.Column<string>(type: "text", nullable: true),
                    LesionesFisicas = table.Column<string>(type: "text", nullable: true),
                    GravedadPercibida = table.Column<string>(type: "text", nullable: true),
                    AtencionMedica = table.Column<string>(type: "text", nullable: true),
                    OtrasPersonasAfectadas = table.Column<string>(type: "text", nullable: true),
                    AfectacionMovilidad = table.Column<string>(type: "text", nullable: true),
                    DanosMateriales = table.Column<string>(type: "text", nullable: true),
                    TipoDanoMaterial = table.Column<string>(type: "text", nullable: true),
                    PerdidaAccesoCuentas = table.Column<string>(type: "text", nullable: true),
                    InformoAutoridad = table.Column<string>(type: "text", nullable: true),
                    AutoridadCanal = table.Column<string>(type: "text", nullable: true),
                    EstadoDenunciaFormal = table.Column<string>(type: "text", nullable: true),
                    TieneRadicado = table.Column<string>(type: "text", nullable: true),
                    NumeroRadicado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NotificoBanco = table.Column<string>(type: "text", nullable: true),
                    RecuperacionActuacion = table.Column<string>(type: "text", nullable: true),
                    IcrPuntaje = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    IcrClasificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reporte", x => x.Id);
                    table.CheckConstraint("CK_Reporte_Estado", "\"EstadoReporte\" IN ('BORRADOR', 'ENVIADO')");
                    table.ForeignKey(
                        name: "FK_Reporte_CuentasUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "CuentasUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reporte_TerritorioBarrios_BarrioId",
                        column: x => x.BarrioId,
                        principalTable: "TerritorioBarrios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reporte_TerritorioMunicipios_MunicipioCodigo",
                        column: x => x.MunicipioCodigo,
                        principalTable: "TerritorioMunicipios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReporteAccesoForzado",
                columns: table => new
                {
                    ReporteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteAccesoForzado", x => new { x.ReporteId, x.Codigo });
                    table.ForeignKey(
                        name: "FK_ReporteAccesoForzado_Reporte_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteBien",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReporteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoriaBien = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<string>(type: "text", nullable: false),
                    ValorAproximado = table.Column<string>(type: "text", nullable: false),
                    Recuperado = table.Column<string>(type: "text", nullable: true),
                    InformacionPersonal = table.Column<string>(type: "text", nullable: true),
                    BloqueoReportado = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteBien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReporteBien_Reporte_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteCaracteristicaSector",
                columns: table => new
                {
                    ReporteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteCaracteristicaSector", x => new { x.ReporteId, x.Codigo });
                    table.ForeignKey(
                        name: "FK_ReporteCaracteristicaSector_Reporte_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteDatosPersonales",
                columns: table => new
                {
                    ReporteId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoParticipacion = table.Column<string>(type: "text", nullable: false),
                    ConsentimientoAceptado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaConsentimiento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    CedulaCifrada = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Celular = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteDatosPersonales", x => x.ReporteId);
                    table.ForeignKey(
                        name: "FK_ReporteDatosPersonales_Reporte_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteIcr",
                columns: table => new
                {
                    ReporteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Criterio = table.Column<string>(type: "text", nullable: false),
                    Puntaje = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteIcr", x => new { x.ReporteId, x.Criterio });
                    table.ForeignKey(
                        name: "FK_ReporteIcr_Reporte_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteResponsables",
                columns: table => new
                {
                    ReporteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CantidadResponsables = table.Column<string>(type: "text", nullable: true),
                    ActuaronCoordinados = table.Column<string>(type: "text", nullable: true),
                    MedioHuida = table.Column<string>(type: "text", nullable: true),
                    RostroObservado = table.Column<string>(type: "text", nullable: true),
                    VestimentaObservada = table.Column<string>(type: "text", nullable: true),
                    VehiculoObservado = table.Column<string>(type: "text", nullable: true),
                    TipoVehiculo = table.Column<string>(type: "text", nullable: true),
                    PlacaObservada = table.Column<string>(type: "text", nullable: true),
                    CaracteristicaDistintiva = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteResponsables", x => x.ReporteId);
                    table.ForeignKey(
                        name: "FK_ReporteResponsables_Reporte_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteBienCelular",
                columns: table => new
                {
                    ReporteBienId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoDispositivo = table.Column<string>(type: "text", nullable: true),
                    SistemaOperativo = table.Column<string>(type: "text", nullable: true),
                    EstadoBloqueo = table.Column<string>(type: "text", nullable: true),
                    UsoAppsBancarias = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteBienCelular", x => x.ReporteBienId);
                    table.ForeignKey(
                        name: "FK_ReporteBienCelular_ReporteBien_ReporteBienId",
                        column: x => x.ReporteBienId,
                        principalTable: "ReporteBien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteBienContenidoBolso",
                columns: table => new
                {
                    ReporteBienId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteBienContenidoBolso", x => new { x.ReporteBienId, x.Codigo });
                    table.ForeignKey(
                        name: "FK_ReporteBienContenidoBolso_ReporteBien_ReporteBienId",
                        column: x => x.ReporteBienId,
                        principalTable: "ReporteBien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteBienDocumento",
                columns: table => new
                {
                    ReporteBienId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteBienDocumento", x => x.ReporteBienId);
                    table.ForeignKey(
                        name: "FK_ReporteBienDocumento_ReporteBien_ReporteBienId",
                        column: x => x.ReporteBienId,
                        principalTable: "ReporteBien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteBienTarjeta",
                columns: table => new
                {
                    ReporteBienId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    TransaccionesNoReconocidas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteBienTarjeta", x => x.ReporteBienId);
                    table.ForeignKey(
                        name: "FK_ReporteBienTarjeta_ReporteBien_ReporteBienId",
                        column: x => x.ReporteBienId,
                        principalTable: "ReporteBien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReporteBienVehiculo",
                columns: table => new
                {
                    ReporteBienId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoVehiculo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Modalidad = table.Column<string>(type: "text", nullable: true),
                    Recuperacion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteBienVehiculo", x => x.ReporteBienId);
                    table.ForeignKey(
                        name: "FK_ReporteBienVehiculo_ReporteBien_ReporteBienId",
                        column: x => x.ReporteBienId,
                        principalTable: "ReporteBien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CatalogoOpciones",
                columns: new[] { "Id", "Catalogo", "Codigo", "CuentaComoDato", "Etiqueta", "Habilitado", "Orden" },
                values: new object[,]
                {
                    { 100, "AccesoForzado", "PUERTA", true, "Puerta", true, 1 },
                    { 101, "AccesoForzado", "VENTANA", true, "Ventana", true, 2 },
                    { 102, "AccesoForzado", "CERRADURA", true, "Cerradura", true, 3 },
                    { 103, "AccesoForzado", "REJA", true, "Reja", true, 4 },
                    { 104, "AccesoForzado", "ACCESO_VEHICULAR", true, "Acceso vehicular", true, 5 },
                    { 105, "AccesoForzado", "OTRO_ACCESO", false, "Otro acceso", true, 6 },
                    { 106, "AccesoForzado", "NINGUNO", true, "Ninguno", true, 7 },
                    { 107, "AccesoForzado", "NO_SABE", false, "No sabe", true, 8 },
                    { 108, "AfectacionMovilidad", "SI", true, "Sí", true, 1 },
                    { 109, "AfectacionMovilidad", "NO", true, "No", true, 2 },
                    { 110, "AfectacionMovilidad", "NO_SABE", false, "No sabe", true, 3 },
                    { 111, "AfectacionMovilidad", "NO_APLICA", false, "No aplica", true, 4 },
                    { 112, "AfluenciaPersonas", "BAJA", true, "Baja", true, 1 },
                    { 113, "AfluenciaPersonas", "MEDIA", true, "Media", true, 2 },
                    { 114, "AfluenciaPersonas", "ALTA", true, "Alta", true, 3 },
                    { 115, "AfluenciaPersonas", "DESCONOCIDA", false, "Desconocida", true, 4 },
                    { 116, "AtencionMedica", "SI", true, "Sí", true, 1 },
                    { 117, "AtencionMedica", "NO", true, "No", true, 2 },
                    { 118, "AtencionMedica", "PENDIENTE", true, "Pendiente", true, 3 },
                    { 119, "AtencionMedica", "PREFIERE_NO_RESPONDER", false, "Prefiere no responder", true, 4 },
                    { 120, "AutoridadCanal", "POLICIA", true, "Policía", true, 1 },
                    { 121, "AutoridadCanal", "FISCALIA", true, "Fiscalía", true, 2 },
                    { 122, "AutoridadCanal", "LINEA_EMERGENCIA", true, "Línea de emergencia", true, 3 },
                    { 123, "AutoridadCanal", "ADMINISTRACION_LUGAR", true, "Administración del lugar", true, 4 },
                    { 124, "AutoridadCanal", "EMPRESA_SEGURIDAD", true, "Empresa de seguridad", true, 5 },
                    { 125, "AutoridadCanal", "OTRO_CANAL", false, "Otro canal", true, 6 },
                    { 126, "AutoridadCanal", "NO_APLICA", false, "No aplica", true, 7 },
                    { 127, "BloqueoReportadoBien", "SI", true, "Sí", true, 1 },
                    { 128, "BloqueoReportadoBien", "NO", true, "No", true, 2 },
                    { 129, "BloqueoReportadoBien", "EN_PROCESO", true, "En proceso", true, 3 },
                    { 130, "BloqueoReportadoBien", "NO_SABE", false, "No sabe", true, 4 },
                    { 131, "BloqueoReportadoBien", "NO_APLICA", false, "No aplica", true, 5 },
                    { 132, "CantidadBien", "1", true, "1", true, 1 },
                    { 133, "CantidadBien", "2", true, "2", true, 2 },
                    { 134, "CantidadBien", "3", true, "3", true, 3 },
                    { 135, "CantidadBien", "4", true, "4", true, 4 },
                    { 136, "CantidadBien", "5_O_MAS", true, "5 o más", true, 5 },
                    { 137, "CantidadBien", "DESCONOCIDA", false, "Desconocida", true, 6 },
                    { 138, "CantidadResponsables", "1", true, "1", true, 1 },
                    { 139, "CantidadResponsables", "2", true, "2", true, 2 },
                    { 140, "CantidadResponsables", "3", true, "3", true, 3 },
                    { 141, "CantidadResponsables", "4_O_MAS", true, "4 o más", true, 4 },
                    { 142, "CantidadResponsables", "NO_SABE", false, "No sabe", true, 5 },
                    { 143, "CaracteristicaDistintiva", "SI", true, "Sí", true, 1 },
                    { 144, "CaracteristicaDistintiva", "NO", true, "No", true, 2 },
                    { 145, "CaracteristicaDistintiva", "NO_RECUERDA", false, "No recuerda", true, 3 },
                    { 146, "CaracteristicaSector", "ZONA_RESIDENCIAL", true, "Zona residencial", true, 1 },
                    { 147, "CaracteristicaSector", "COMERCIAL", true, "Comercial", true, 2 },
                    { 148, "CaracteristicaSector", "INDUSTRIAL", true, "Industrial", true, 3 },
                    { 149, "CaracteristicaSector", "EDUCATIVA", true, "Educativa", true, 4 },
                    { 150, "CaracteristicaSector", "RECREATIVA", true, "Recreativa", true, 5 },
                    { 151, "CaracteristicaSector", "FINANCIERA", true, "Financiera", true, 6 },
                    { 152, "CaracteristicaSector", "MIXTA", true, "Mixta", true, 7 },
                    { 153, "CaracteristicaSector", "NO_IDENTIFICADO", false, "No identificado", true, 8 },
                    { 154, "CategoriaBien", "CELULAR", true, "Celular", true, 1 },
                    { 155, "CategoriaBien", "COMPUTADOR_TABLETA", true, "Computador o tableta", true, 2 },
                    { 156, "CategoriaBien", "RELOJ_ACCESORIO", true, "Reloj o accesorio", true, 3 },
                    { 157, "CategoriaBien", "BOLSO_MORRAL_CARTERA", true, "Bolso, morral o cartera", true, 4 },
                    { 158, "CategoriaBien", "DINERO_EFECTIVO", true, "Dinero en efectivo", true, 5 },
                    { 159, "CategoriaBien", "TARJETA_BANCARIA", true, "Tarjeta bancaria", true, 6 },
                    { 160, "CategoriaBien", "DOCUMENTOS_PERSONALES", true, "Documentos personales", true, 7 },
                    { 161, "CategoriaBien", "JOYAS", true, "Joyas", true, 8 },
                    { 162, "CategoriaBien", "BICICLETA", true, "Bicicleta", true, 9 },
                    { 163, "CategoriaBien", "MOTOCICLETA", true, "Motocicleta", true, 10 },
                    { 164, "CategoriaBien", "AUTOMOVIL", true, "Automóvil", true, 11 },
                    { 165, "CategoriaBien", "PARTES_VEHICULO", true, "Partes de vehículo", true, 12 },
                    { 166, "CategoriaBien", "ELECTRODOMESTICOS", true, "Electrodomésticos", true, 13 },
                    { 167, "CategoriaBien", "HERRAMIENTAS", true, "Herramientas", true, 14 },
                    { 168, "CategoriaBien", "MERCANCIA", true, "Mercancía", true, 15 },
                    { 169, "CategoriaBien", "ROPA_CALZADO", true, "Ropa o calzado", true, 16 },
                    { 170, "CategoriaBien", "EQUIPOS_TRABAJO", true, "Equipos de trabajo", true, 17 },
                    { 171, "CategoriaBien", "OTROS_BIENES", false, "Otros bienes", true, 18 },
                    { 172, "CategoriaBien", "NO_APLICA", false, "No aplica", true, 19 },
                    { 173, "CategoriaPrincipal", "HURTO_PERSONA", true, "Hurto a persona", true, 1 },
                    { 174, "CategoriaPrincipal", "HURTO_VIVIENDA", true, "Hurto a vivienda", true, 2 },
                    { 175, "CategoriaPrincipal", "HURTO_ESTABLECIMIENTO_COMERCIAL", true, "Hurto a establecimiento comercial", true, 3 },
                    { 176, "CategoriaPrincipal", "HURTO_VEHICULO", true, "Hurto de vehículo", true, 4 },
                    { 177, "CategoriaPrincipal", "HURTO_PARTES_VEHICULO", true, "Hurto de partes o accesorios de vehículo", true, 5 },
                    { 178, "CategoriaPrincipal", "HURTO_BICICLETA_MOVILIDAD", true, "Hurto de bicicleta o vehículo de movilidad personal", true, 6 },
                    { 179, "CategoriaPrincipal", "HURTO_TRANSPORTE_PUBLICO", true, "Hurto en transporte público", true, 7 },
                    { 180, "CategoriaPrincipal", "HURTO_ESPACIO_PUBLICO", true, "Hurto en infraestructura o espacio público", true, 8 },
                    { 181, "CategoriaPrincipal", "OTRA_CATEGORIA", false, "Otra categoría", true, 9 },
                    { 182, "CondicionClima", "SOLEADO", true, "Soleado", true, 1 },
                    { 183, "CondicionClima", "PARCIALMENTE_NUBLADO", true, "Parcialmente nublado", true, 2 },
                    { 184, "CondicionClima", "NUBLADO", true, "Nublado", true, 3 },
                    { 185, "CondicionClima", "LLUVIA_LIGERA", true, "Lluvia ligera", true, 4 },
                    { 186, "CondicionClima", "LLUVIA_FUERTE", true, "Lluvia fuerte", true, 5 },
                    { 187, "CondicionClima", "TORMENTA_ELECTRICA", true, "Tormenta eléctrica", true, 6 },
                    { 188, "CondicionClima", "NEBLINA", true, "Neblina", true, 7 },
                    { 189, "CondicionClima", "OTRO", false, "Otro", true, 8 },
                    { 190, "CondicionClima", "NO_SABE", false, "No sabe", true, 9 },
                    { 191, "ContenidoBolso", "DOCUMENTOS", true, "Documentos", true, 1 },
                    { 192, "ContenidoBolso", "DINERO", true, "Dinero", true, 2 },
                    { 193, "ContenidoBolso", "TARJETAS", true, "Tarjetas", true, 3 },
                    { 194, "ContenidoBolso", "CELULAR", true, "Celular", true, 4 },
                    { 195, "ContenidoBolso", "LLAVES", true, "Llaves", true, 5 },
                    { 196, "ContenidoBolso", "OBJETOS_PERSONALES", true, "Objetos personales", true, 6 },
                    { 197, "ContenidoBolso", "CONTENIDO_DESCONOCIDO", false, "Contenido desconocido", true, 7 },
                    { 198, "ContextoIncidente", "CAMINANDO", true, "Caminando", true, 1 },
                    { 199, "ContextoIncidente", "DESPLAZANDOSE_VEHICULO", true, "Desplazándose en vehículo", true, 2 },
                    { 200, "ContextoIncidente", "TRANSPORTE_PUBLICO", true, "Usando transporte público", true, 3 },
                    { 201, "ContextoIncidente", "DENTRO_VIVIENDA", true, "Dentro de una vivienda", true, 4 },
                    { 202, "ContextoIncidente", "DENTRO_ESTABLECIMIENTO", true, "Dentro de un establecimiento", true, 5 },
                    { 203, "ContextoIncidente", "ESPACIO_PUBLICO", true, "En un espacio público", true, 6 },
                    { 204, "ContextoIncidente", "OTRO_CONTEXTO", false, "En otro contexto", true, 7 },
                    { 205, "DanoMaterial", "PUERTA", true, "Puerta", true, 1 },
                    { 206, "DanoMaterial", "VENTANA", true, "Ventana", true, 2 },
                    { 207, "DanoMaterial", "CERRADURA", true, "Cerradura", true, 3 },
                    { 208, "DanoMaterial", "VEHICULO", true, "Vehículo", true, 4 },
                    { 209, "DanoMaterial", "MOBILIARIO", true, "Mobiliario", true, 5 },
                    { 210, "DanoMaterial", "INFRAESTRUCTURA", true, "Infraestructura", true, 6 },
                    { 211, "DanoMaterial", "OTRO", false, "Otro", true, 7 },
                    { 212, "DanoMaterial", "NO_APLICA", false, "No aplica", true, 8 },
                    { 213, "EntornoLugar", "URBANO", true, "Urbano", true, 1 },
                    { 214, "EntornoLugar", "RURAL", true, "Rural", true, 2 },
                    { 215, "EntornoLugar", "NO_IDENTIFICADO", false, "No identificado", true, 3 },
                    { 216, "EstadoBloqueoCelular", "BLOQUEADO", true, "Bloqueado", true, 1 },
                    { 217, "EstadoBloqueoCelular", "PENDIENTE_BLOQUEO", true, "Pendiente de bloqueo", true, 2 },
                    { 218, "EstadoBloqueoCelular", "NO_BLOQUEADO", true, "No bloqueado", true, 3 },
                    { 219, "EstadoBloqueoCelular", "NO_SABE", false, "No sabe", true, 4 },
                    { 220, "EstadoDenunciaFormal", "PRESENTADA", true, "Presentada", true, 1 },
                    { 221, "EstadoDenunciaFormal", "PENDIENTE_PRESENTACION", true, "Pendiente de presentación", true, 2 },
                    { 222, "EstadoDenunciaFormal", "NO_PRESENTADA", true, "No presentada", true, 3 },
                    { 223, "EstadoDenunciaFormal", "NO_SABE", false, "No sabe", true, 4 },
                    { 224, "EstadoDenunciaFormal", "NO_APLICA", false, "No aplica", true, 5 },
                    { 225, "EstadoDocumento", "PERDIDO_O_ROBADO", true, "Reportados como perdidos o robados", true, 1 },
                    { 226, "EstadoDocumento", "EN_REPOSICION", true, "En proceso de reposición", true, 2 },
                    { 227, "EstadoDocumento", "REPUESTO", true, "Repuestos", true, 3 },
                    { 228, "EstadoDocumento", "PENDIENTE", false, "Pendiente", true, 4 },
                    { 229, "EstadoTarjeta", "BLOQUEADA", true, "Bloqueada", true, 1 },
                    { 230, "EstadoTarjeta", "PENDIENTE_BLOQUEO", true, "Pendiente de bloqueo", true, 2 },
                    { 231, "EstadoTarjeta", "NO_BLOQUEADA", true, "No bloqueada", true, 3 },
                    { 232, "EstadoTarjeta", "NO_SABE", false, "No sabe", true, 4 },
                    { 233, "EstadoVehiculoBien", "ESTACIONADO", true, "Estacionado", true, 1 },
                    { 234, "EstadoVehiculoBien", "EN_CIRCULACION", true, "En circulación", true, 2 },
                    { 235, "EstadoVehiculoBien", "OTRO", false, "Otro", true, 3 },
                    { 236, "FranjaHoraria", "MADRUGADA", true, "Madrugada (00:00 a 05:59)", true, 1 },
                    { 237, "FranjaHoraria", "MANANA", true, "Mañana (06:00 a 11:59)", true, 2 },
                    { 238, "FranjaHoraria", "TARDE", true, "Tarde (12:00 a 17:59)", true, 3 },
                    { 239, "FranjaHoraria", "NOCHE", true, "Noche (18:00 a 23:59)", true, 4 },
                    { 240, "GravedadPercibida", "SIN_LESIONES", true, "Sin lesiones", true, 1 },
                    { 241, "GravedadPercibida", "LEVES", true, "Leves", true, 2 },
                    { 242, "GravedadPercibida", "REQUIRIERON_ATENCION_MEDICA", true, "Requirieron atención médica", true, 3 },
                    { 243, "GravedadPercibida", "GRAVES", true, "Graves", true, 4 },
                    { 244, "GravedadPercibida", "NO_SABE", false, "No sabe", true, 5 },
                    { 245, "GravedadPercibida", "PREFIERE_NO_RESPONDER", false, "Prefiere no responder", true, 6 },
                    { 246, "IluminacionLugar", "LUZ_NATURAL", true, "Luz natural", true, 1 },
                    { 247, "IluminacionLugar", "ALUMBRADO_ADECUADO", true, "Alumbrado público adecuado", true, 2 },
                    { 248, "IluminacionLugar", "ALUMBRADO_DEFICIENTE", true, "Alumbrado deficiente", true, 3 },
                    { 249, "IluminacionLugar", "SIN_ILUMINACION", true, "Sin iluminación", true, 4 },
                    { 250, "IluminacionLugar", "NO_OBSERVADA", false, "Iluminación no observada", true, 5 },
                    { 251, "InformacionPersonalBien", "SI", true, "Sí", true, 1 },
                    { 252, "InformacionPersonalBien", "NO", true, "No", true, 2 },
                    { 253, "InformacionPersonalBien", "NO_SABE", false, "No sabe", true, 3 },
                    { 254, "InformacionPersonalBien", "NO_APLICA", false, "No aplica", true, 4 },
                    { 255, "InformoAutoridad", "SI", true, "Sí", true, 1 },
                    { 256, "InformoAutoridad", "NO", true, "No", true, 2 },
                    { 257, "InformoAutoridad", "EN_PROCESO", true, "En proceso", true, 3 },
                    { 258, "InformoAutoridad", "NO_SABE", false, "No sabe", true, 4 },
                    { 259, "InformoAutoridad", "PREFIERE_NO_RESPONDER", false, "Prefiere no responder", true, 5 },
                    { 260, "IngresoInmueble", "CON_AUTORIZACION_INICIAL", true, "Sí, con autorización inicial", true, 1 },
                    { 261, "IngresoInmueble", "SIN_AUTORIZACION", true, "Sí, sin autorización", true, 2 },
                    { 262, "IngresoInmueble", "NO", true, "No", true, 3 },
                    { 263, "IngresoInmueble", "NO_APLICA", false, "No aplica", true, 4 },
                    { 264, "IngresoInmueble", "NO_SABE", false, "No sabe", true, 5 },
                    { 265, "IntensidadLluvia", "SIN_LLUVIA", true, "Sin lluvia", true, 1 },
                    { 266, "IntensidadLluvia", "LIGERA", true, "Ligera", true, 2 },
                    { 267, "IntensidadLluvia", "MODERADA", true, "Moderada", true, 3 },
                    { 268, "IntensidadLluvia", "FUERTE", true, "Fuerte", true, 4 },
                    { 269, "IntensidadLluvia", "NO_SABE", false, "No sabe", true, 5 },
                    { 270, "LesionesFisicas", "SI", true, "Sí", true, 1 },
                    { 271, "LesionesFisicas", "NO", true, "No", true, 2 },
                    { 272, "LesionesFisicas", "NO_SABE", false, "No sabe", true, 3 },
                    { 273, "LesionesFisicas", "PREFIERE_NO_RESPONDER", false, "Prefiere no responder", true, 4 },
                    { 274, "MedioAproximacion", "A_PIE", true, "A pie", true, 1 },
                    { 275, "MedioAproximacion", "MOTOCICLETA", true, "Motocicleta", true, 2 },
                    { 276, "MedioAproximacion", "AUTOMOVIL", true, "Automóvil", true, 3 },
                    { 277, "MedioAproximacion", "BICICLETA", true, "Bicicleta", true, 4 },
                    { 278, "MedioAproximacion", "TRANSPORTE_PUBLICO", true, "Transporte público", true, 5 },
                    { 279, "MedioAproximacion", "OTRO_VEHICULO", true, "Otro vehículo", true, 6 },
                    { 280, "MedioAproximacion", "NO_IDENTIFICADO", false, "No se identificó", true, 7 },
                    { 281, "MedioAproximacion", "NO_APLICA", false, "No aplica", true, 8 },
                    { 282, "MedioHuida", "A_PIE", true, "A pie", true, 1 },
                    { 283, "MedioHuida", "MOTOCICLETA", true, "Motocicleta", true, 2 },
                    { 284, "MedioHuida", "AUTOMOVIL", true, "Automóvil", true, 3 },
                    { 285, "MedioHuida", "BICICLETA", true, "Bicicleta", true, 4 },
                    { 286, "MedioHuida", "TRANSPORTE_PUBLICO", true, "Transporte público", true, 5 },
                    { 287, "MedioHuida", "OTRO", false, "Otro", true, 6 },
                    { 288, "MedioHuida", "NO_IDENTIFICADO", false, "No identificado", true, 7 },
                    { 289, "MedioHuida", "NO_HUBO_HUIDA_OBSERVADA", true, "No hubo huida observada", true, 8 },
                    { 290, "ModalidadPercibida", "ARREBATO", true, "Arrebato", true, 1 },
                    { 291, "ModalidadPercibida", "RAPONAZO", true, "Raponazo", true, 2 },
                    { 292, "ModalidadPercibida", "COSQUILLEO_SUSTRACCION", true, "Cosquilleo o sustracción sin advertencia", true, 3 },
                    { 293, "ModalidadPercibida", "INTIMIDACION", true, "Intimidación", true, 4 },
                    { 294, "ModalidadPercibida", "AMENAZA_CON_ARMA", true, "Amenaza con arma", true, 5 },
                    { 295, "ModalidadPercibida", "FUERZA_FISICA", true, "Fuerza física", true, 6 },
                    { 296, "ModalidadPercibida", "INGRESO_NO_AUTORIZADO_INMUEBLE", true, "Ingreso no autorizado a inmueble", true, 7 },
                    { 297, "ModalidadPercibida", "FORZAMIENTO_CERRADURA_ACCESO", true, "Forzamiento de cerradura o acceso", true, 8 },
                    { 298, "ModalidadPercibida", "ENGANO_DISTRACCION", true, "Engaño o distracción", true, 9 },
                    { 299, "ModalidadPercibida", "SUSTRACCION_OBJETO_DESATENDIDO", true, "Sustracción de objeto desatendido", true, 10 },
                    { 300, "ModalidadPercibida", "ACCESO_NO_AUTORIZADO_VEHICULO", true, "Acceso no autorizado a vehículo", true, 11 },
                    { 301, "ModalidadPercibida", "NO_IDENTIFICADA", false, "Modalidad no identificada", true, 12 },
                    { 302, "ModalidadVehiculoBien", "ROBO_COMPLETO", true, "Robo completo", true, 1 },
                    { 303, "ModalidadVehiculoBien", "INTENTO_ROBO", true, "Intento de robo", true, 2 },
                    { 304, "ModalidadVehiculoBien", "SUSTRACCION_ACCESORIOS", true, "Sustracción de accesorios", true, 3 },
                    { 305, "MomentoIntervencion", "SIN_INTERACCION_PREVIA", true, "El hecho ocurrió sin interacción previa", true, 1 },
                    { 306, "MomentoIntervencion", "ACERCAMIENTO_CONVERSACION", true, "Hubo acercamiento o conversación", true, 2 },
                    { 307, "MomentoIntervencion", "FORCEJEO", true, "Hubo forcejeo", true, 3 },
                    { 308, "MomentoIntervencion", "PERSECUCION", true, "Hubo persecución", true, 4 },
                    { 309, "MomentoIntervencion", "NO_SABE", false, "No sabe", true, 5 },
                    { 310, "NotificoBanco", "SI", true, "Sí", true, 1 },
                    { 311, "NotificoBanco", "NO", true, "No", true, 2 },
                    { 312, "NotificoBanco", "EN_PROCESO", true, "En proceso", true, 3 },
                    { 313, "NotificoBanco", "NO_APLICA", false, "No aplica", true, 4 },
                    { 314, "PerdidaAccesoCuentas", "SI", true, "Sí", true, 1 },
                    { 315, "PerdidaAccesoCuentas", "NO", true, "No", true, 2 },
                    { 316, "PerdidaAccesoCuentas", "NO_SABE", false, "No sabe", true, 3 },
                    { 317, "PerdidaAccesoCuentas", "NO_APLICA", false, "No aplica", true, 4 },
                    { 318, "PlacaObservada", "COMPLETA", true, "Completa", true, 1 },
                    { 319, "PlacaObservada", "PARCIAL", true, "Parcial", true, 2 },
                    { 320, "PlacaObservada", "NO", true, "No", true, 3 },
                    { 321, "PlacaObservada", "NO_SABE", false, "No sabe", true, 4 },
                    { 322, "PrecisionTemporal", "HORA_EXACTA", true, "Hora exacta", true, 1 },
                    { 323, "PrecisionTemporal", "FRANJA_APROXIMADA", true, "Franja aproximada", true, 2 },
                    { 324, "PrecisionTemporal", "MOMENTO_DESCONOCIDO", false, "Momento desconocido", true, 3 },
                    { 325, "RecuperacionActuacion", "SI_TODOS", true, "Sí, todos", true, 1 },
                    { 326, "RecuperacionActuacion", "SI_ALGUNOS", true, "Sí, algunos", true, 2 },
                    { 327, "RecuperacionActuacion", "NO", true, "No", true, 3 },
                    { 328, "RecuperacionActuacion", "NO_SABE", false, "No sabe", true, 4 },
                    { 329, "RecuperacionActuacion", "NO_APLICA", false, "No aplica", true, 5 },
                    { 330, "RecuperacionVehiculo", "RECUPERADO", true, "Recuperado", true, 1 },
                    { 331, "RecuperacionVehiculo", "NO_RECUPERADO", true, "No recuperado", true, 2 },
                    { 332, "RecuperacionVehiculo", "PARCIALMENTE_RECUPERADO", true, "Parcialmente recuperado", true, 3 },
                    { 333, "RecuperacionVehiculo", "NO_SABE", false, "No sabe", true, 4 },
                    { 334, "RecuperadoBien", "SI_TOTAL", true, "Sí, totalmente", true, 1 },
                    { 335, "RecuperadoBien", "SI_PARCIAL", true, "Sí, parcialmente", true, 2 },
                    { 336, "RecuperadoBien", "NO", true, "No", true, 3 },
                    { 337, "RecuperadoBien", "NO_SABE", false, "No sabe", true, 4 },
                    { 338, "RelacionLugar", "VICTIMA_RESIDENTE", true, "Víctima residente", true, 1 },
                    { 339, "RelacionLugar", "VISITANTE", true, "Visitante", true, 2 },
                    { 340, "RelacionLugar", "TRABAJADOR", true, "Trabajador", true, 3 },
                    { 341, "RelacionLugar", "CLIENTE", true, "Cliente", true, 4 },
                    { 342, "RelacionLugar", "TRANSEUNTE", true, "Transeúnte", true, 5 },
                    { 343, "RelacionLugar", "NO_APLICA", false, "No aplica", true, 6 },
                    { 344, "RelacionLugar", "NO_SABE", false, "No sabe", true, 7 },
                    { 345, "ResultadoHecho", "SE_LLEVARON_BIENES", true, "Se llevaron bienes", true, 1 },
                    { 346, "ResultadoHecho", "INTENTARON_NO_LOGRARON", true, "Intentaron llevarse bienes pero no lo lograron", true, 2 },
                    { 347, "ResultadoHecho", "RECUPERARON_TODOS", true, "Recuperaron todos los bienes", true, 3 },
                    { 348, "ResultadoHecho", "RECUPERARON_PARTE", true, "Recuperaron parte de los bienes", true, 4 },
                    { 349, "ResultadoHecho", "RESULTADO_DESCONOCIDO", false, "Resultado desconocido", true, 5 },
                    { 350, "RostroObservado", "SI", true, "Sí", true, 1 },
                    { 351, "RostroObservado", "PARCIALMENTE", true, "Parcialmente", true, 2 },
                    { 352, "RostroObservado", "NO", true, "No", true, 3 },
                    { 353, "RostroObservado", "NO_SABE", false, "No sabe", true, 4 },
                    { 354, "SiNoNoSabe", "SI", true, "Sí", true, 1 },
                    { 355, "SiNoNoSabe", "NO", true, "No", true, 2 },
                    { 356, "SiNoNoSabe", "NO_SABE", false, "No sabe", true, 3 },
                    { 357, "SistemaOperativo", "ANDROID", true, "Android", true, 1 },
                    { 358, "SistemaOperativo", "IOS", true, "iOS", true, 2 },
                    { 359, "SistemaOperativo", "OTRO", false, "Otro", true, 3 },
                    { 360, "SistemaOperativo", "NO_SABE", false, "No sabe", true, 4 },
                    { 361, "TieneRadicado", "SI", true, "Sí", true, 1 },
                    { 362, "TieneRadicado", "NO", true, "No", true, 2 },
                    { 363, "TieneRadicado", "NO_SABE", false, "No sabe", true, 3 },
                    { 364, "TieneRadicado", "NO_APLICA", false, "No aplica", true, 4 },
                    { 365, "TipoArma", "ARMA_FUEGO", true, "Arma de fuego", true, 1 },
                    { 366, "TipoArma", "ARMA_CORTOPUNZANTE", true, "Arma cortopunzante", true, 2 },
                    { 367, "TipoArma", "OBJETO_CONTUNDENTE", true, "Objeto contundente", true, 3 },
                    { 368, "TipoArma", "OBJETO_INTIMIDAR", true, "Objeto usado para intimidar", true, 4 },
                    { 369, "TipoArma", "ARMA_NO_IDENTIFICADA", false, "Arma no identificada", true, 5 },
                    { 370, "TipoDispositivo", "TELEFONO_INTELIGENTE", true, "Teléfono inteligente", true, 1 },
                    { 371, "TipoDispositivo", "TELEFONO_BASICO", true, "Teléfono básico", true, 2 },
                    { 372, "TipoDispositivo", "OTRO_DISPOSITIVO", false, "Otro dispositivo móvil", true, 3 },
                    { 373, "TipoDocumento", "DOCUMENTO_IDENTIDAD", true, "Documento de identidad", true, 1 },
                    { 374, "TipoDocumento", "LICENCIA_CONDUCCION", true, "Licencia de conducción", true, 2 },
                    { 375, "TipoDocumento", "PASAPORTE", true, "Pasaporte", true, 3 },
                    { 376, "TipoDocumento", "DOCUMENTOS_BANCARIOS", true, "Documentos bancarios", true, 4 },
                    { 377, "TipoDocumento", "OTROS_DOCUMENTOS", false, "Otros documentos", true, 5 },
                    { 378, "TipoIncidente", "ROBO_CONSUMADO", true, "Robo consumado", true, 1 },
                    { 379, "TipoIncidente", "INTENTO_SIN_SUSTRACCION", true, "Intento de robo sin sustracción", true, 2 },
                    { 380, "TipoIncidente", "INTENTO_SUSTRACCION_PARCIAL", true, "Intento con sustracción parcial", true, 3 },
                    { 381, "TipoIncidente", "NO_CLASIFICADO", false, "No está seguro de cómo clasificarlo", true, 4 },
                    { 382, "TipoLugar", "VIA_PUBLICA", true, "Vía pública", true, 1 },
                    { 383, "TipoLugar", "ANDEN", true, "Andén", true, 2 },
                    { 384, "TipoLugar", "PARQUE", true, "Parque", true, 3 },
                    { 385, "TipoLugar", "ESTACION_TRANSPORTE", true, "Estación de transporte", true, 4 },
                    { 386, "TipoLugar", "VEHICULO_TRANSPORTE_PUBLICO", true, "Vehículo o transporte público", true, 5 },
                    { 387, "TipoLugar", "VIVIENDA", true, "Vivienda", true, 6 },
                    { 388, "TipoLugar", "UNIDAD_RESIDENCIAL", true, "Unidad residencial", true, 7 },
                    { 389, "TipoLugar", "ESTABLECIMIENTO_COMERCIAL", true, "Establecimiento comercial", true, 8 },
                    { 390, "TipoLugar", "OFICINA", true, "Oficina", true, 9 },
                    { 391, "TipoLugar", "INSTITUCION_EDUCATIVA", true, "Institución educativa", true, 10 },
                    { 392, "TipoLugar", "INSTITUCION_SALUD", true, "Institución de salud", true, 11 },
                    { 393, "TipoLugar", "CAJERO_ENTIDAD_FINANCIERA", true, "Cajero o entidad financiera", true, 12 },
                    { 394, "TipoLugar", "CENTRO_COMERCIAL", true, "Centro comercial", true, 13 },
                    { 395, "TipoLugar", "ESPACIO_DEPORTIVO", true, "Espacio deportivo", true, 14 },
                    { 396, "TipoLugar", "OTRO_LUGAR_CATEGORIZADO", false, "Otro lugar categorizado", true, 15 },
                    { 397, "TipoParticipacion", "VICTIMA_DIRECTA", true, "Víctima directa", true, 1 },
                    { 398, "TipoParticipacion", "TESTIGO", true, "Testigo", true, 2 },
                    { 399, "TipoParticipacion", "FAMILIAR_ACOMPANANTE", true, "Familiar o acompañante", true, 3 },
                    { 400, "TipoParticipacion", "OTRA_PERSONA", true, "Otra persona que reporta", true, 4 },
                    { 401, "TipoTarjeta", "DEBITO", true, "Débito", true, 1 },
                    { 402, "TipoTarjeta", "CREDITO", true, "Crédito", true, 2 },
                    { 403, "TipoTarjeta", "PREPAGO", true, "Prepago", true, 3 },
                    { 404, "TipoTarjeta", "NO_SABE", false, "No sabe", true, 4 },
                    { 405, "TipoUbicacion", "GPS_ACTUAL", true, "GPS actual", true, 1 },
                    { 406, "TipoUbicacion", "PUNTO_MAPA", true, "Seleccionar punto en mapa", true, 2 },
                    { 407, "TipoUbicacion", "UBICACION_APROXIMADA", true, "Ubicación aproximada", true, 3 },
                    { 408, "TipoUbicacion", "UBICACION_PREVIA", true, "Ubicación ingresada previamente", true, 4 },
                    { 409, "TipoVehiculoBien", "AUTOMOVIL", true, "Automóvil", true, 1 },
                    { 410, "TipoVehiculoBien", "MOTOCICLETA", true, "Motocicleta", true, 2 },
                    { 411, "TipoVehiculoBien", "BICICLETA", true, "Bicicleta", true, 3 },
                    { 412, "TipoVehiculoBien", "OTRO_VEHICULO", true, "Otro vehículo", true, 4 },
                    { 413, "TipoVehiculoObservado", "MOTOCICLETA", true, "Motocicleta", true, 1 },
                    { 414, "TipoVehiculoObservado", "AUTOMOVIL", true, "Automóvil", true, 2 },
                    { 415, "TipoVehiculoObservado", "CAMIONETA", true, "Camioneta", true, 3 },
                    { 416, "TipoVehiculoObservado", "BICICLETA", true, "Bicicleta", true, 4 },
                    { 417, "TipoVehiculoObservado", "PATINETA_ELECTRICA", true, "Patineta eléctrica", true, 5 },
                    { 418, "TipoVehiculoObservado", "OTRO", false, "Otro", true, 6 },
                    { 419, "TipoVehiculoObservado", "NO_IDENTIFICADO", false, "No identificado", true, 7 },
                    { 420, "TransaccionesNoReconocidas", "SI", true, "Sí", true, 1 },
                    { 421, "TransaccionesNoReconocidas", "NO", true, "No", true, 2 },
                    { 422, "TransaccionesNoReconocidas", "PENDIENTE_VERIFICAR", false, "Pendiente de verificar", true, 3 },
                    { 423, "UsoAppsBancarias", "SI", true, "Sí", true, 1 },
                    { 424, "UsoAppsBancarias", "NO", true, "No", true, 2 },
                    { 425, "UsoAppsBancarias", "PREFIERE_NO_RESPONDER", false, "Prefiere no responder", true, 3 },
                    { 426, "ValorBien", "SIN_VALOR_ESTIMADO", true, "Sin valor estimado", true, 1 },
                    { 427, "ValorBien", "MENOS_100000", true, "Menos de $100.000", true, 2 },
                    { 428, "ValorBien", "100000_499999", true, "$100.000 a $499.999", true, 3 },
                    { 429, "ValorBien", "500000_999999", true, "$500.000 a $999.999", true, 4 },
                    { 430, "ValorBien", "1000000_4999999", true, "$1.000.000 a $4.999.999", true, 5 },
                    { 431, "ValorBien", "5000000_9999999", true, "$5.000.000 a $9.999.999", true, 6 },
                    { 432, "ValorBien", "10000000_O_MAS", true, "$10.000.000 o más", true, 7 },
                    { 433, "ValorBien", "NO_SABE", false, "No sabe", true, 8 },
                    { 434, "VestimentaObservada", "SI", true, "Sí", true, 1 },
                    { 435, "VestimentaObservada", "PARCIALMENTE", true, "Parcialmente", true, 2 },
                    { 436, "VestimentaObservada", "NO", true, "No", true, 3 },
                    { 437, "VestimentaObservada", "NO_SABE", false, "No sabe", true, 4 },
                    { 438, "VisibilidadEntorno", "BUENA", true, "Buena", true, 1 },
                    { 439, "VisibilidadEntorno", "REDUCIDA", true, "Reducida", true, 2 },
                    { 440, "VisibilidadEntorno", "MUY_REDUCIDA", true, "Muy reducida", true, 3 },
                    { 441, "VisibilidadEntorno", "NO_SABE", false, "No sabe", true, 4 }
                });

            migrationBuilder.InsertData(
                table: "CriteriosIcr",
                columns: new[] { "Codigo", "Activo", "Nombre", "Peso" },
                values: new object[,]
                {
                    { "COMPLETITUD", true, "Completitud", 25m },
                    { "CONSISTENCIA", true, "Consistencia y validez", 5m },
                    { "DENUNCIA", true, "Denuncia", 10m },
                    { "RADICADO", true, "Radicado", 10m },
                    { "TIEMPO", true, "Tiempo transcurrido", 30m },
                    { "UBICACION", true, "Precisión de la ubicación", 20m }
                });

            migrationBuilder.InsertData(
                table: "TerritorioMunicipios",
                columns: new[] { "Codigo", "Nombre" },
                values: new object[,]
                {
                    { "BARBOSA", "Barbosa" },
                    { "BELLO", "Bello" },
                    { "CALDAS", "Caldas" },
                    { "COPACABANA", "Copacabana" },
                    { "ENVIGADO", "Envigado" },
                    { "GIRARDOTA", "Girardota" },
                    { "ITAGUI", "Itagüí" },
                    { "LA_ESTRELLA", "La Estrella" },
                    { "MEDELLIN", "Medellín" },
                    { "SABANETA", "Sabaneta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reporte_BarrioId",
                table: "Reporte",
                column: "BarrioId");

            migrationBuilder.CreateIndex(
                name: "IX_Reporte_MunicipioCodigo",
                table: "Reporte",
                column: "MunicipioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Reporte_UsuarioId_FechaActualizacion",
                table: "Reporte",
                columns: new[] { "UsuarioId", "FechaActualizacion" });

            migrationBuilder.CreateIndex(
                name: "IX_ReporteBien_ReporteId",
                table: "ReporteBien",
                column: "ReporteId");

            migrationBuilder.CreateIndex(
                name: "IX_TerritorioBarrios_ComunaId",
                table: "TerritorioBarrios",
                column: "ComunaId");

            migrationBuilder.CreateIndex(
                name: "IX_TerritorioBarrios_MunicipioCodigo",
                table: "TerritorioBarrios",
                column: "MunicipioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_TerritorioBarrios_Nombre",
                table: "TerritorioBarrios",
                column: "Nombre");
            // Columnas espaciales y de cálculo
            migrationBuilder.Sql("""
                ALTER TABLE "Reporte" ADD COLUMN "Ubicacion" geography(Point, 4326) GENERATED ALWAYS AS (CASE WHEN "Latitud" IS NULL OR "Longitud" IS NULL THEN NULL ELSE ST_SetSRID(ST_MakePoint("Longitud", "Latitud"), 4326)::geography END) STORED;
                CREATE INDEX "IX_Reporte_Ubicacion" ON "Reporte" USING GIST ("Ubicacion");
                ALTER TABLE "TerritorioComunas" ADD COLUMN "Geom" geometry(MultiPolygon, 4326);
                ALTER TABLE "TerritorioBarrios" ADD COLUMN "Geom" geometry(MultiPolygon, 4326);
                ALTER TABLE "CoberturaMetropolitana" ADD COLUMN "Geom" geometry(MultiPolygon, 4326);
                CREATE INDEX "IX_TerritorioBarrios_Geom" ON "TerritorioBarrios" USING GIST ("Geom");
                CREATE INDEX "IX_CoberturaMetropolitana_Geom" ON "CoberturaMetropolitana" USING GIST ("Geom");
                """);

            // Cobertura del área metropolitana
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION fn_validar_cobertura(p_latitud DOUBLE PRECISION, p_longitud DOUBLE PRECISION)
                RETURNS BOOLEAN LANGUAGE sql STABLE AS $$
                    SELECT EXISTS (
                        SELECT 1 FROM "CoberturaMetropolitana" m
                        WHERE ST_Covers(m."Geom", ST_SetSRID(ST_MakePoint(p_longitud, p_latitud), 4326))
                    );
                $$;
                """);

            // Los criterios del ICR suman 100
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION fn_validar_suma_pesos_icr()
                RETURNS TRIGGER LANGUAGE plpgsql AS $$
                DECLARE
                    v_total NUMERIC;
                BEGIN
                    SELECT COALESCE(SUM("Peso"), 0) INTO v_total FROM "CriteriosIcr" WHERE "Activo";
                    IF v_total <> 100 THEN
                        RAISE EXCEPTION 'La suma de pesos del ICR es %, debe ser 100', v_total;
                    END IF;
                    RETURN NULL;
                END;
                $$;
                CREATE CONSTRAINT TRIGGER trg_validar_suma_pesos_icr
                AFTER INSERT OR UPDATE OR DELETE ON "CriteriosIcr"
                DEFERRABLE INITIALLY DEFERRED
                FOR EACH ROW EXECUTE FUNCTION fn_validar_suma_pesos_icr();
                """);

            // Un reporte enviado no se modifica ni se borra
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION fn_proteger_reporte_enviado()
                RETURNS TRIGGER LANGUAGE plpgsql AS $$
                BEGIN
                    IF OLD."EstadoReporte" = 'ENVIADO' THEN
                        IF TG_OP = 'DELETE' THEN
                            RAISE EXCEPTION 'Un reporte enviado no se puede borrar';
                        END IF;
                        RAISE EXCEPTION 'Un reporte enviado no se puede modificar';
                    END IF;
                    IF TG_OP = 'DELETE' THEN
                        RETURN OLD;
                    END IF;
                    RETURN NEW;
                END;
                $$;
                CREATE TRIGGER trg_proteger_reporte_enviado
                BEFORE UPDATE OR DELETE ON "Reporte"
                FOR EACH ROW EXECUTE FUNCTION fn_proteger_reporte_enviado();
                """);

            // Limpieza de borradores vencidos
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION fn_eliminar_borradores_vencidos()
                RETURNS INT LANGUAGE plpgsql AS $$
                DECLARE
                    v_eliminados INT;
                BEGIN
                    DELETE FROM "Reporte"
                    WHERE "EstadoReporte" = 'BORRADOR' AND "FechaVencimientoBorrador" < NOW();
                    GET DIAGNOSTICS v_eliminados = ROW_COUNT;
                    RETURN v_eliminados;
                END;
                $$;
                """);

            // Seguridad de filas (RLS) del reporte y del territorio
            migrationBuilder.Sql("""
                ALTER TABLE "Reporte" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_Reporte_propio ON "Reporte" FOR ALL USING (fn_modo_sistema() OR "UsuarioId" = fn_usuario_actual()) WITH CHECK (fn_modo_sistema() OR "UsuarioId" = fn_usuario_actual());
                ALTER TABLE "ReporteBien" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteBien_propio ON "ReporteBien" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteResponsables" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteResponsables_propio ON "ReporteResponsables" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteDatosPersonales" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteDatosPersonales_propio ON "ReporteDatosPersonales" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteCaracteristicaSector" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteCaracteristicaSector_propio ON "ReporteCaracteristicaSector" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteAccesoForzado" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteAccesoForzado_propio ON "ReporteAccesoForzado" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteIcr" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteIcr_propio ON "ReporteIcr" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "Reporte" r WHERE r."Id" = "ReporteId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteBienCelular" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteBienCelular_propio ON "ReporteBienCelular" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteBienVehiculo" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteBienVehiculo_propio ON "ReporteBienVehiculo" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteBienTarjeta" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteBienTarjeta_propio ON "ReporteBienTarjeta" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteBienDocumento" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteBienDocumento_propio ON "ReporteBienDocumento" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "ReporteBienContenidoBolso" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_ReporteBienContenidoBolso_propio ON "ReporteBienContenidoBolso" FOR ALL USING (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual())) WITH CHECK (fn_modo_sistema() OR EXISTS (SELECT 1 FROM "ReporteBien" b JOIN "Reporte" r ON r."Id" = b."ReporteId" WHERE b."Id" = "ReporteBienId" AND r."UsuarioId" = fn_usuario_actual()));
                ALTER TABLE "TerritorioMunicipios" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_TerritorioMunicipios_lectura ON "TerritorioMunicipios" FOR SELECT USING (true);
                CREATE POLICY pol_TerritorioMunicipios_escritura ON "TerritorioMunicipios" FOR ALL USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());
                ALTER TABLE "TerritorioComunas" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_TerritorioComunas_lectura ON "TerritorioComunas" FOR SELECT USING (true);
                CREATE POLICY pol_TerritorioComunas_escritura ON "TerritorioComunas" FOR ALL USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());
                ALTER TABLE "TerritorioBarrios" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_TerritorioBarrios_lectura ON "TerritorioBarrios" FOR SELECT USING (true);
                CREATE POLICY pol_TerritorioBarrios_escritura ON "TerritorioBarrios" FOR ALL USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());
                ALTER TABLE "CoberturaMetropolitana" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_CoberturaMetropolitana_lectura ON "CoberturaMetropolitana" FOR SELECT USING (true);
                CREATE POLICY pol_CoberturaMetropolitana_escritura ON "CoberturaMetropolitana" FOR ALL USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());
                ALTER TABLE "CriteriosIcr" ENABLE ROW LEVEL SECURITY;
                CREATE POLICY pol_CriteriosIcr_lectura ON "CriteriosIcr" FOR SELECT USING (true);
                CREATE POLICY pol_CriteriosIcr_escritura ON "CriteriosIcr" FOR ALL USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());
                """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {            // Retirar seguridad de filas y funciones del reporte
            migrationBuilder.Sql("""
                DROP POLICY IF EXISTS pol_Reporte_propio ON "Reporte";
                ALTER TABLE "Reporte" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteBien_propio ON "ReporteBien";
                ALTER TABLE "ReporteBien" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteResponsables_propio ON "ReporteResponsables";
                ALTER TABLE "ReporteResponsables" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteDatosPersonales_propio ON "ReporteDatosPersonales";
                ALTER TABLE "ReporteDatosPersonales" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteCaracteristicaSector_propio ON "ReporteCaracteristicaSector";
                ALTER TABLE "ReporteCaracteristicaSector" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteAccesoForzado_propio ON "ReporteAccesoForzado";
                ALTER TABLE "ReporteAccesoForzado" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteIcr_propio ON "ReporteIcr";
                ALTER TABLE "ReporteIcr" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteBienCelular_propio ON "ReporteBienCelular";
                ALTER TABLE "ReporteBienCelular" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteBienVehiculo_propio ON "ReporteBienVehiculo";
                ALTER TABLE "ReporteBienVehiculo" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteBienTarjeta_propio ON "ReporteBienTarjeta";
                ALTER TABLE "ReporteBienTarjeta" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteBienDocumento_propio ON "ReporteBienDocumento";
                ALTER TABLE "ReporteBienDocumento" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_ReporteBienContenidoBolso_propio ON "ReporteBienContenidoBolso";
                ALTER TABLE "ReporteBienContenidoBolso" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_TerritorioMunicipios_lectura ON "TerritorioMunicipios";
                DROP POLICY IF EXISTS pol_TerritorioMunicipios_escritura ON "TerritorioMunicipios";
                ALTER TABLE "TerritorioMunicipios" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_TerritorioComunas_lectura ON "TerritorioComunas";
                DROP POLICY IF EXISTS pol_TerritorioComunas_escritura ON "TerritorioComunas";
                ALTER TABLE "TerritorioComunas" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_TerritorioBarrios_lectura ON "TerritorioBarrios";
                DROP POLICY IF EXISTS pol_TerritorioBarrios_escritura ON "TerritorioBarrios";
                ALTER TABLE "TerritorioBarrios" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_CoberturaMetropolitana_lectura ON "CoberturaMetropolitana";
                DROP POLICY IF EXISTS pol_CoberturaMetropolitana_escritura ON "CoberturaMetropolitana";
                ALTER TABLE "CoberturaMetropolitana" DISABLE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS pol_CriteriosIcr_lectura ON "CriteriosIcr";
                DROP POLICY IF EXISTS pol_CriteriosIcr_escritura ON "CriteriosIcr";
                ALTER TABLE "CriteriosIcr" DISABLE ROW LEVEL SECURITY;
                DROP TRIGGER IF EXISTS trg_proteger_reporte_enviado ON "Reporte";
                DROP FUNCTION IF EXISTS fn_proteger_reporte_enviado();
                DROP TRIGGER IF EXISTS trg_validar_suma_pesos_icr ON "CriteriosIcr";
                DROP FUNCTION IF EXISTS fn_validar_suma_pesos_icr();
                DROP FUNCTION IF EXISTS fn_eliminar_borradores_vencidos();
                DROP FUNCTION IF EXISTS fn_validar_cobertura(DOUBLE PRECISION, DOUBLE PRECISION);
                """);


            migrationBuilder.DropTable(
                name: "CoberturaMetropolitana");

            migrationBuilder.DropTable(
                name: "CriteriosIcr");

            migrationBuilder.DropTable(
                name: "ReporteAccesoForzado");

            migrationBuilder.DropTable(
                name: "ReporteBienCelular");

            migrationBuilder.DropTable(
                name: "ReporteBienContenidoBolso");

            migrationBuilder.DropTable(
                name: "ReporteBienDocumento");

            migrationBuilder.DropTable(
                name: "ReporteBienTarjeta");

            migrationBuilder.DropTable(
                name: "ReporteBienVehiculo");

            migrationBuilder.DropTable(
                name: "ReporteCaracteristicaSector");

            migrationBuilder.DropTable(
                name: "ReporteDatosPersonales");

            migrationBuilder.DropTable(
                name: "ReporteIcr");

            migrationBuilder.DropTable(
                name: "ReporteResponsables");

            migrationBuilder.DropTable(
                name: "ReporteBien");

            migrationBuilder.DropTable(
                name: "Reporte");

            migrationBuilder.DropTable(
                name: "TerritorioBarrios");

            migrationBuilder.DropTable(
                name: "TerritorioComunas");

            migrationBuilder.DropTable(
                name: "TerritorioMunicipios");

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 157);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 163);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 164);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 165);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 166);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 167);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 168);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 171);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 172);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 173);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 174);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 175);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 176);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 177);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 178);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 179);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 180);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 181);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 182);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 183);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 184);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 185);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 186);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 187);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 188);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 189);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 190);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 191);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 192);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 193);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 194);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 195);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 196);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 197);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 198);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 199);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 212);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 213);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 214);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 215);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 216);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 217);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 218);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 219);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 220);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 221);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 222);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 223);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 224);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 225);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 226);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 227);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 228);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 229);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 230);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 231);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 232);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 233);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 234);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 235);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 236);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 237);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 238);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 239);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 240);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 241);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 242);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 243);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 244);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 245);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 246);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 247);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 248);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 249);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 250);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 251);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 252);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 253);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 254);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 255);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 256);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 257);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 258);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 259);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 260);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 261);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 262);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 263);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 264);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 265);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 266);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 267);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 268);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 269);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 270);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 271);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 272);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 273);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 274);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 275);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 276);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 277);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 278);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 279);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 280);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 281);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 282);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 283);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 284);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 285);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 286);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 287);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 288);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 289);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 290);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 291);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 292);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 293);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 294);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 295);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 296);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 297);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 298);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 299);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 300);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 301);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 302);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 303);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 304);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 305);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 306);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 307);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 308);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 309);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 310);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 311);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 312);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 313);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 314);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 315);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 316);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 317);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 318);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 319);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 320);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 321);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 322);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 323);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 324);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 325);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 326);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 327);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 328);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 329);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 330);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 331);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 332);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 333);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 334);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 335);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 336);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 337);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 338);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 339);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 340);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 341);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 342);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 343);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 344);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 345);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 346);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 347);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 348);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 349);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 350);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 351);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 352);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 353);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 354);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 355);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 356);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 357);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 358);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 359);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 360);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 361);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 362);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 363);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 364);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 365);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 366);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 367);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 368);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 369);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 370);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 371);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 372);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 373);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 374);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 375);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 376);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 377);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 378);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 379);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 380);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 381);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 382);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 383);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 384);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 385);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 386);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 387);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 388);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 389);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 390);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 391);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 392);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 393);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 394);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 395);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 396);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 397);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 398);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 399);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 400);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 401);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 402);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 403);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 404);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 405);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 406);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 407);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 408);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 409);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 410);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 411);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 412);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 413);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 414);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 415);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 416);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 417);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 418);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 419);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 420);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 421);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 422);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 423);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 424);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 425);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 426);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 427);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 428);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 429);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 430);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 431);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 432);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 433);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 434);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 435);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 436);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 437);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 438);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 439);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 440);

            migrationBuilder.DeleteData(
                table: "CatalogoOpciones",
                keyColumn: "Id",
                keyValue: 441);
        }
    }
}
