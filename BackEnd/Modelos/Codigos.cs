namespace AppRobos.Backend.Modelos;

// Códigos de los estados de cuenta. Deben coincidir con la tabla EstadosCuenta.
public static class EstadosCuentaCodigos
{
    public const string Activa = "ACTIVA";
    public const string Inactiva = "INACTIVA";
    public const string Bloqueada = "BLOQUEADA";
}

// Nombres de los roles de la aplicación.
// Ciudadano se asigna al registrarse. SuperAdmin solo se asigna desde la base de datos.
public static class RolesAplicacion
{
    public const string Ciudadano = "Ciudadano";
    public const string SuperAdmin = "SuperAdmin";
}

// Nombres de las listas (catálogos) guardadas en la tabla CatalogoOpciones.
public static class CatalogosAplicacion
{
    public const string SexoGenero = "SexoGenero";
}
