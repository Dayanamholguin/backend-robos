namespace AppRobos.Backend.Modelos;

// Opción de una lista desplegable (por ejemplo, SexoGenero: MASCULINO, FEMENINO, PREFIERO_NO_DECIR).
// Tabla: CatalogoOpciones. La app envía el código y el backend lo valida contra esta tabla.
public class CatalogoOpcion
{
    // Identificador interno.
    public int Id { get; set; }

    // Nombre de la lista, por ejemplo SexoGenero.
    public string Catalogo { get; set; } = string.Empty;

    // Valor que envía la app, por ejemplo MASCULINO.
    public string Codigo { get; set; } = string.Empty;

    // Texto que ve el ciudadano, por ejemplo "Masculino".
    public string Etiqueta { get; set; } = string.Empty;

    // Posición en la lista desplegable.
    public int Orden { get; set; }

    // Si es FALSE, la opción deja de aceptarse sin borrarla.
    public bool Habilitado { get; set; } = true;

    // FALSE para respuestas que no suman puntaje en el ICR (no sabe, otro, no aplica, etc.).
    public bool CuentaComoDato { get; set; } = true;
}
