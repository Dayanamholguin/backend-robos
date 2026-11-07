using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppRobos.Backend.Seguridad;

// Lectura del ciudadano autenticado a partir de los claims del token.
public static class ExtensionesSesion
{
    // Identificador de la cuenta (claim "sub"). Null si el token no lo trae o no es válido.
    public static Guid? ObtenerIdCuenta(this ClaimsPrincipal principal)
    {
        var texto = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(texto, out var id) ? id : null;
    }
}
