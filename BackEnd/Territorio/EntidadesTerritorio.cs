namespace AppRobos.Backend.Territorio;

// Municipio del Valle de Aburrá que puede elegir el ciudadano. Tabla: TerritorioMunicipios.
public class TerritorioMunicipio
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

// Comuna de Medellín con su polígono. Tabla: TerritorioComunas.
public class TerritorioComuna
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

// Barrio o sector con su polígono. Tabla: TerritorioBarrios.
// ComunaId solo aplica a Medellín; en los demás municipios queda vacío.
public class TerritorioBarrio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string MunicipioCodigo { get; set; } = string.Empty;
    public int? ComunaId { get; set; }

    public TerritorioMunicipio? Municipio { get; set; }
    public TerritorioComuna? Comuna { get; set; }
}

// Polígono del área metropolitana. Solo se aceptan ubicaciones dentro de él.
// Tabla: CoberturaMetropolitana.
public class CoberturaMetropolitana
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
