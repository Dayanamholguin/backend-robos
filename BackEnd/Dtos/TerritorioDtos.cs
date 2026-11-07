using System.ComponentModel.DataAnnotations;

namespace AppRobos.Backend.Dtos;

// Coordenadas que la app envía al tocar el mapa.
public class UbicacionConsultaDto
{
    [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
    public double Latitud { get; set; }

    [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
    public double Longitud { get; set; }
}

// Municipio y barrio que corresponden a un punto del mapa.
// Si el punto no tiene barrio cargado (por ejemplo, un municipio sin barrios importados),
// BarrioId y BarrioNombre llegan vacíos y Aviso explica por qué.
public class UbicacionTerritorialRespuestaDto
{
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public string? MunicipioCodigo { get; set; }
    public string? MunicipioNombre { get; set; }
    public int? BarrioId { get; set; }
    public string? BarrioNombre { get; set; }
    public string? Aviso { get; set; }
}
