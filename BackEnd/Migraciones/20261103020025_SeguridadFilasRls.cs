using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppRobos.Backend.Migraciones
{
    // <inheritdoc />
    public partial class SeguridadFilasRls : Migration
    {
        // <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Funciones que leen la sesión que establece la API en cada conexión
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION fn_usuario_actual() RETURNS uuid LANGUAGE sql STABLE AS $$ SELECT NULLIF(current_setting('app.usuario_id', true), '')::uuid $$;");
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION fn_modo_sistema() RETURNS boolean LANGUAGE sql STABLE AS $$ SELECT COALESCE(NULLIF(current_setting('app.modo_sistema', true), ''), 'off') = 'on' $$;");
            // Tabla CuentasUsuario: cada fila es de un ciudadano; solo ve y cambia sus propias filas
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuario"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_CuentasUsuario_propia ON ""CuentasUsuario"" FOR ALL USING (fn_modo_sistema() OR ""Id"" = fn_usuario_actual()) WITH CHECK (fn_modo_sistema() OR ""Id"" = fn_usuario_actual());");
            // Tabla CuentasUsuarioRoles: cada fila es de un ciudadano; solo ve y cambia sus propias filas
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuarioRoles"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_CuentasUsuarioRoles_propia ON ""CuentasUsuarioRoles"" FOR ALL USING (fn_modo_sistema() OR ""UserId"" = fn_usuario_actual()) WITH CHECK (fn_modo_sistema() OR ""UserId"" = fn_usuario_actual());");
            // Tabla CuentasUsuarioClaims: cada fila es de un ciudadano; solo ve y cambia sus propias filas
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuarioClaims"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_CuentasUsuarioClaims_propia ON ""CuentasUsuarioClaims"" FOR ALL USING (fn_modo_sistema() OR ""UserId"" = fn_usuario_actual()) WITH CHECK (fn_modo_sistema() OR ""UserId"" = fn_usuario_actual());");
            // Tabla CuentasUsuarioLogins: cada fila es de un ciudadano; solo ve y cambia sus propias filas
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuarioLogins"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_CuentasUsuarioLogins_propia ON ""CuentasUsuarioLogins"" FOR ALL USING (fn_modo_sistema() OR ""UserId"" = fn_usuario_actual()) WITH CHECK (fn_modo_sistema() OR ""UserId"" = fn_usuario_actual());");
            // Tabla CuentasUsuarioTokens: cada fila es de un ciudadano; solo ve y cambia sus propias filas
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuarioTokens"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_CuentasUsuarioTokens_propia ON ""CuentasUsuarioTokens"" FOR ALL USING (fn_modo_sistema() OR ""UserId"" = fn_usuario_actual()) WITH CHECK (fn_modo_sistema() OR ""UserId"" = fn_usuario_actual());");
            // Tabla RolesAplicacion: todos pueden leerla; solo el sistema la cambia
            migrationBuilder.Sql(@"ALTER TABLE ""RolesAplicacion"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_RolesAplicacion_lectura ON ""RolesAplicacion"" FOR SELECT USING (true);");
            migrationBuilder.Sql(@"CREATE POLICY pol_RolesAplicacion_insertar ON ""RolesAplicacion"" FOR INSERT WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_RolesAplicacion_actualizar ON ""RolesAplicacion"" FOR UPDATE USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_RolesAplicacion_borrar ON ""RolesAplicacion"" FOR DELETE USING (fn_modo_sistema());");
            // Tabla RolesAplicacionClaims: todos pueden leerla; solo el sistema la cambia
            migrationBuilder.Sql(@"ALTER TABLE ""RolesAplicacionClaims"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_RolesAplicacionClaims_lectura ON ""RolesAplicacionClaims"" FOR SELECT USING (true);");
            migrationBuilder.Sql(@"CREATE POLICY pol_RolesAplicacionClaims_insertar ON ""RolesAplicacionClaims"" FOR INSERT WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_RolesAplicacionClaims_actualizar ON ""RolesAplicacionClaims"" FOR UPDATE USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_RolesAplicacionClaims_borrar ON ""RolesAplicacionClaims"" FOR DELETE USING (fn_modo_sistema());");
            // Tabla EstadosCuenta: todos pueden leerla; solo el sistema la cambia
            migrationBuilder.Sql(@"ALTER TABLE ""EstadosCuenta"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_EstadosCuenta_lectura ON ""EstadosCuenta"" FOR SELECT USING (true);");
            migrationBuilder.Sql(@"CREATE POLICY pol_EstadosCuenta_insertar ON ""EstadosCuenta"" FOR INSERT WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_EstadosCuenta_actualizar ON ""EstadosCuenta"" FOR UPDATE USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_EstadosCuenta_borrar ON ""EstadosCuenta"" FOR DELETE USING (fn_modo_sistema());");
            // Tabla CatalogoOpciones: todos pueden leerla; solo el sistema la cambia
            migrationBuilder.Sql(@"ALTER TABLE ""CatalogoOpciones"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_CatalogoOpciones_lectura ON ""CatalogoOpciones"" FOR SELECT USING (true);");
            migrationBuilder.Sql(@"CREATE POLICY pol_CatalogoOpciones_insertar ON ""CatalogoOpciones"" FOR INSERT WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_CatalogoOpciones_actualizar ON ""CatalogoOpciones"" FOR UPDATE USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_CatalogoOpciones_borrar ON ""CatalogoOpciones"" FOR DELETE USING (fn_modo_sistema());");
            // Tabla PoliticasLimiteTasa: todos pueden leerla; solo el sistema la cambia
            migrationBuilder.Sql(@"ALTER TABLE ""PoliticasLimiteTasa"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY pol_PoliticasLimiteTasa_lectura ON ""PoliticasLimiteTasa"" FOR SELECT USING (true);");
            migrationBuilder.Sql(@"CREATE POLICY pol_PoliticasLimiteTasa_insertar ON ""PoliticasLimiteTasa"" FOR INSERT WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_PoliticasLimiteTasa_actualizar ON ""PoliticasLimiteTasa"" FOR UPDATE USING (fn_modo_sistema()) WITH CHECK (fn_modo_sistema());");
            migrationBuilder.Sql(@"CREATE POLICY pol_PoliticasLimiteTasa_borrar ON ""PoliticasLimiteTasa"" FOR DELETE USING (fn_modo_sistema());");
        }

        // <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CuentasUsuario_propia ON ""CuentasUsuario"";");
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuario"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CuentasUsuarioRoles_propia ON ""CuentasUsuarioRoles"";");
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuarioRoles"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CuentasUsuarioClaims_propia ON ""CuentasUsuarioClaims"";");
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuarioClaims"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CuentasUsuarioLogins_propia ON ""CuentasUsuarioLogins"";");
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuarioLogins"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CuentasUsuarioTokens_propia ON ""CuentasUsuarioTokens"";");
            migrationBuilder.Sql(@"ALTER TABLE ""CuentasUsuarioTokens"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_RolesAplicacion_lectura ON ""RolesAplicacion"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_RolesAplicacion_insertar ON ""RolesAplicacion"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_RolesAplicacion_actualizar ON ""RolesAplicacion"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_RolesAplicacion_borrar ON ""RolesAplicacion"";");
            migrationBuilder.Sql(@"ALTER TABLE ""RolesAplicacion"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_RolesAplicacionClaims_lectura ON ""RolesAplicacionClaims"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_RolesAplicacionClaims_insertar ON ""RolesAplicacionClaims"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_RolesAplicacionClaims_actualizar ON ""RolesAplicacionClaims"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_RolesAplicacionClaims_borrar ON ""RolesAplicacionClaims"";");
            migrationBuilder.Sql(@"ALTER TABLE ""RolesAplicacionClaims"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_EstadosCuenta_lectura ON ""EstadosCuenta"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_EstadosCuenta_insertar ON ""EstadosCuenta"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_EstadosCuenta_actualizar ON ""EstadosCuenta"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_EstadosCuenta_borrar ON ""EstadosCuenta"";");
            migrationBuilder.Sql(@"ALTER TABLE ""EstadosCuenta"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CatalogoOpciones_lectura ON ""CatalogoOpciones"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CatalogoOpciones_insertar ON ""CatalogoOpciones"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CatalogoOpciones_actualizar ON ""CatalogoOpciones"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_CatalogoOpciones_borrar ON ""CatalogoOpciones"";");
            migrationBuilder.Sql(@"ALTER TABLE ""CatalogoOpciones"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_PoliticasLimiteTasa_lectura ON ""PoliticasLimiteTasa"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_PoliticasLimiteTasa_insertar ON ""PoliticasLimiteTasa"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_PoliticasLimiteTasa_actualizar ON ""PoliticasLimiteTasa"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS pol_PoliticasLimiteTasa_borrar ON ""PoliticasLimiteTasa"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PoliticasLimiteTasa"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS fn_modo_sistema();");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS fn_usuario_actual();");
        }
    }
}
