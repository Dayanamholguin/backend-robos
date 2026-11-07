using System.ComponentModel.DataAnnotations;

namespace AppRobos.Backend.Validaciones;

// Valida la fecha de nacimiento: debe ser anterior a hoy y no puede ser anterior a 1900.
// Evita fechas futuras o imposibles en el registro.
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class FechaNacimientoValidaAttribute : ValidationAttribute
{
    // Año mínimo aceptado para la fecha de nacimiento.
    private static readonly DateOnly FechaMinima = new(1900, 1, 1);

    public FechaNacimientoValidaAttribute()
        : base("La fecha de nacimiento debe ser anterior a hoy y posterior al año 1900.")
    {
    }

    // Devuelve válido si la fecha está entre 1900 y hoy (excluyendo hoy).
    public override bool IsValid(object? value)
    {
        if (value is not DateOnly fecha)
        {
            return false;
        }

        // Se compara con la fecha actual en UTC para que la regla no dependa del servidor
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        return fecha >= FechaMinima && fecha < hoy;
    }
}
