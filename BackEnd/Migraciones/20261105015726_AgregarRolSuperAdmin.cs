using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppRobos.Backend.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarRolSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RolesAplicacion",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("b3e8d1f0-4a6c-4e2b-8d7f-9c1a5e3b2d64"), "rol-superadmin-inicial", "SuperAdmin", "SUPERADMIN" });

            // Indica si la sesión actual es de un SuperAdmin. Lo fija la API al abrir cada conexión.
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION fn_es_superadmin() RETURNS boolean LANGUAGE sql STABLE AS $$ SELECT COALESCE(NULLIF(current_setting('app.es_superadmin', true), ''), 'off') = 'on' $$;");

            // El SuperAdmin solo puede leer. Estas políticas se suman a las de cada ciudadano (son permisivas).
            foreach (var tabla in TablasReporte)
            {
                migrationBuilder.Sql($@"CREATE POLICY pol_{tabla}_superadmin_lectura ON ""{tabla}"" FOR SELECT USING (fn_es_superadmin());");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var tabla in TablasReporte)
            {
                migrationBuilder.Sql($@"DROP POLICY IF EXISTS pol_{tabla}_superadmin_lectura ON ""{tabla}"";");
            }

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_es_superadmin();");

            migrationBuilder.DeleteData(
                table: "RolesAplicacion",
                keyColumn: "Id",
                keyValue: new Guid("b3e8d1f0-4a6c-4e2b-8d7f-9c1a5e3b2d64"));
        }

        // Tablas del reporte ciudadano que el SuperAdmin puede leer.
        private static readonly string[] TablasReporte =
        {
            "Reporte",
            "ReporteBien",
            "ReporteBienCelular",
            "ReporteBienVehiculo",
            "ReporteBienTarjeta",
            "ReporteBienDocumento",
            "ReporteBienContenidoBolso",
            "ReporteResponsables",
            "ReporteDatosPersonales",
            "ReporteCaracteristicaSector",
            "ReporteAccesoForzado",
            "ReporteIcr",
        };
    }
}
