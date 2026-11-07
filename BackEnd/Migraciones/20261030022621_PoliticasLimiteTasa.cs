using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppRobos.Backend.Migraciones
{
    // <inheritdoc />
    public partial class PoliticasLimiteTasa : Migration
    {
        // <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoliticasLimiteTasa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LimitePeticiones = table.Column<int>(type: "integer", nullable: false),
                    VentanaSegundos = table.Column<int>(type: "integer", nullable: false),
                    EstrategiaParticion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LimiteCola = table.Column<int>(type: "integer", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliticasLimiteTasa", x => x.Id);
                    table.CheckConstraint("CK_PoliticasLimiteTasa_Cola", "\"LimiteCola\" = 0");
                    table.CheckConstraint("CK_PoliticasLimiteTasa_Limite", "\"LimitePeticiones\" BETWEEN 1 AND 10000");
                    table.CheckConstraint("CK_PoliticasLimiteTasa_Ventana", "\"VentanaSegundos\" BETWEEN 1 AND 3600");
                });

            migrationBuilder.InsertData(
                table: "PoliticasLimiteTasa",
                columns: new[] { "Id", "Activa", "EstrategiaParticion", "FechaActualizacion", "FechaCreacion", "LimiteCola", "LimitePeticiones", "Nombre", "VentanaSegundos" },
                values: new object[,]
                {
                    { 1, true, "IP", new DateTimeOffset(new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 5, "PoliticaLogin", 60 },
                    { 2, true, "IP", new DateTimeOffset(new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 3, "PoliticaRegistro", 60 },
                    { 3, true, "IP", new DateTimeOffset(new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 60, "PoliticaLecturaReportes", 60 },
                    { 4, true, "IP", new DateTimeOffset(new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 100, "PoliticaPorDefecto", 60 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoliticasLimiteTasa_Nombre",
                table: "PoliticasLimiteTasa",
                column: "Nombre",
                unique: true);
        }

        // <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoliticasLimiteTasa");
        }
    }
}
