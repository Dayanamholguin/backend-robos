using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Modelos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AppRobos.Backend.Servicios;

// Token generado junto con su fecha de expiración.
public record TokenGenerado(string Token, DateTimeOffset ExpiraEn);

// Crea tokens JWT firmados con HMAC-SHA256.
// Los claims usan los nombres estándar "sub", "email" y "role" para que coincidan con la validación.
public class ServicioTokenJwt
{
    // Nombre del claim que lleva el sello de seguridad de la cuenta.
    public const string ClaimSello = "sello";

    private readonly ConfiguracionJwt _configuracion;

    public ServicioTokenJwt(IOptions<ConfiguracionJwt> configuracion)
    {
        _configuracion = configuracion.Value;
    }

    // Genera un token para la cuenta con sus roles.
    // Cuenta autenticada.
    // Roles de la cuenta.
    // Momento actual en UTC; de aquí sale la expiración.
    public TokenGenerado CrearToken(CuentaUsuario cuenta, IEnumerable<string> roles, DateTimeOffset ahora)
    {
        var expiraEn = ahora.AddMinutes(_configuracion.MinutosExpiracion);

        // Claims del token: identificador, correo, identificador único del token y roles
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, cuenta.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, cuenta.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // Sello de seguridad: cambia al cambiar la contraseña y así invalida los tokens anteriores
            new(ClaimSello, cuenta.SecurityStamp ?? string.Empty)
        };
        claims.AddRange(roles.Select(rol => new Claim("role", rol)));

        // Firma con la clave configurada (nunca se guarda en el código)
        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracion.Clave));
        var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuracion.Emisor,
            audience: _configuracion.Audiencia,
            claims: claims,
            notBefore: ahora.UtcDateTime,
            expires: expiraEn.UtcDateTime,
            signingCredentials: credenciales);

        return new TokenGenerado(new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }
}
