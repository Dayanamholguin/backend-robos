using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppRobos.Backend.Migraciones
{
    // <inheritdoc />
    public partial class CreacionAutenticacion : Migration
    {
        // <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PostGIS: extensión geoespacial necesaria para las columnas geography y geometry
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS postgis;");

            migrationBuilder.CreateTable(
                name: "CatalogoOpciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Catalogo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Codigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Etiqueta = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Habilitado = table.Column<bool>(type: "boolean", nullable: false),
                    CuentaComoDato = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogoOpciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCuenta",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Habilitado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCuenta", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "RolesAplicacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesAplicacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CuentasUsuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    SexoCodigo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    EstadoCodigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NivelBloqueo = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CuentaDeshabilitada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasUsuario_EstadosCuenta_EstadoCodigo",
                        column: x => x.EstadoCodigo,
                        principalTable: "EstadosCuenta",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolesAplicacionClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesAplicacionClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolesAplicacionClaims_RolesAplicacion_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RolesAplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasUsuarioClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasUsuarioClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasUsuarioClaims_CuentasUsuario_UserId",
                        column: x => x.UserId,
                        principalTable: "CuentasUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasUsuarioLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasUsuarioLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_CuentasUsuarioLogins_CuentasUsuario_UserId",
                        column: x => x.UserId,
                        principalTable: "CuentasUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasUsuarioRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasUsuarioRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_CuentasUsuarioRoles_CuentasUsuario_UserId",
                        column: x => x.UserId,
                        principalTable: "CuentasUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuentasUsuarioRoles_RolesAplicacion_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RolesAplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasUsuarioTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasUsuarioTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_CuentasUsuarioTokens_CuentasUsuario_UserId",
                        column: x => x.UserId,
                        principalTable: "CuentasUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CatalogoOpciones",
                columns: new[] { "Id", "Catalogo", "Codigo", "CuentaComoDato", "Etiqueta", "Habilitado", "Orden" },
                values: new object[,]
                {
                    { 1, "SexoGenero", "MASCULINO", true, "Masculino", true, 1 },
                    { 2, "SexoGenero", "FEMENINO", true, "Femenino", true, 2 },
                    { 3, "SexoGenero", "PREFIERO_NO_DECIR", true, "Prefiero no decirlo", true, 3 }
                });

            migrationBuilder.InsertData(
                table: "EstadosCuenta",
                columns: new[] { "Codigo", "Descripcion", "Habilitado" },
                values: new object[,]
                {
                    { "ACTIVA", "Cuenta activa; puede usar la aplicación.", true },
                    { "BLOQUEADA", "Cuenta bloqueada temporalmente por intentos fallidos.", true },
                    { "INACTIVA", "Cuenta inactiva; no puede iniciar sesión.", true }
                });

            migrationBuilder.InsertData(
                table: "RolesAplicacion",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("6f1c2a4e-1b7d-4c2a-9a1e-5d3f0b8c7a01"), "rol-ciudadano-inicial", "Ciudadano", "CIUDADANO" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogoOpciones_Catalogo_Codigo",
                table: "CatalogoOpciones",
                columns: new[] { "Catalogo", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "CuentasUsuario",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasUsuario_EstadoCodigo",
                table: "CuentasUsuario",
                column: "EstadoCodigo");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "CuentasUsuario",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuentasUsuarioClaims_UserId",
                table: "CuentasUsuarioClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasUsuarioLogins_UserId",
                table: "CuentasUsuarioLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasUsuarioRoles_RoleId",
                table: "CuentasUsuarioRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "RolesAplicacion",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolesAplicacionClaims_RoleId",
                table: "RolesAplicacionClaims",
                column: "RoleId");
        }

        // <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogoOpciones");

            migrationBuilder.DropTable(
                name: "CuentasUsuarioClaims");

            migrationBuilder.DropTable(
                name: "CuentasUsuarioLogins");

            migrationBuilder.DropTable(
                name: "CuentasUsuarioRoles");

            migrationBuilder.DropTable(
                name: "CuentasUsuarioTokens");

            migrationBuilder.DropTable(
                name: "RolesAplicacionClaims");

            migrationBuilder.DropTable(
                name: "CuentasUsuario");

            migrationBuilder.DropTable(
                name: "RolesAplicacion");

            migrationBuilder.DropTable(
                name: "EstadosCuenta");
        }
    }
}
