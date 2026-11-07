using System.ComponentModel.DataAnnotations;

namespace AppRobos.Backend.Validaciones;

// Valida la fecha de nacimiento: la persona debe tener entre 18 y 100 años.
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class FechaNacimientoValidaAttribute : ValidationAttribute
{
    public FechaNacimientoValidaAttribute()
        : base("La fecha de nacimiento debe corresponder a una persona de entre 18 y 100 años.")
    {
    }

    // Devuelve válido si la fecha corresponde a una edad entre 18 y 100 años.
    public override bool IsValid(object? value)
    {
        if (value is not DateOnly fecha)
        {
            return false;
        }

        // Se compara con la fecha actual en UTC para que la regla no dependa del servidor
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var nacioAntesDe100 = hoy.AddYears(-100);
        var cumpleDieciochoAntes = hoy.AddYears(-18);
        return fecha >= nacioAntesDe100 && fecha <= cumpleDieciochoAntes;
    }
}
