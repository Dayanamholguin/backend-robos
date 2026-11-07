using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppRobos.Backend.Migraciones
{
    // <inheritdoc />
    public partial class ValidacionPoliticasLimiteTasa : Migration
    {
        // <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Función que impide desactivar login y registro, y registra la fecha de cambio
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION fn_validar_politica_limite()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    -- Login y registro siempre deben estar protegidos
    IF NEW.""Nombre"" IN ('PoliticaLogin', 'PoliticaRegistro') AND NEW.""Activa"" = FALSE THEN
        RAISE EXCEPTION 'La política % no se puede desactivar', NEW.""Nombre"";
    END IF;

    NEW.""FechaActualizacion"" := NOW();   -- Registra el momento del cambio
    RETURN NEW;
END;
$$;");

            // Trigger que ejecuta la función antes de cada inserción o actualización
            migrationBuilder.Sql(@"
CREATE TRIGGER trg_validar_politica_limite
BEFORE INSERT OR UPDATE ON ""PoliticasLimiteTasa""
FOR EACH ROW EXECUTE FUNCTION fn_validar_politica_limite();");
        }

        // <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_validar_politica_limite ON ""PoliticasLimiteTasa"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS fn_validar_politica_limite();");
        }
    }
}
