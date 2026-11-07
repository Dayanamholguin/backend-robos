using System.IdentityModel.Tokens.Jwt;
using AppRobos.Backend.Modelos;
using AppRobos.Backend.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AppRobos.Backend.Seguridad;

// Revisa cada token válido contra la cuenta en la BD.
// Si el sello del token ya no coincide con el de la cuenta (por ejemplo, se cambió la contraseña),
// si la cuenta fue deshabilitada o si está inactiva, el token se rechaza aunque no haya vencido.
public static class ValidacionSesion
{
    // Evento OnTokenValidated de JwtBearer.
    public static async Task ValidarAsync(TokenValidatedContext contexto)
    {
        var principal = contexto.Principal;
        var idTexto = principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var selloToken = principal?.FindFirst(ServicioTokenJwt.ClaimSello)?.Value;

        if (!Guid.TryParse(idTexto, out var idCuenta) || string.IsNullOrEmpty(selloToken))
        {
            contexto.Fail("El token no identifica una sesión válida.");
            return;
        }

        // Antes de consultar la cuenta, la base debe saber que la petición es de este ciudadano
        var sesionBd = contexto.HttpContext.RequestServices.GetRequiredService<ContextoSesionBaseDatos>();
        sesionBd.EstablecerUsuario(idCuenta);

        var usuarios = contexto.HttpContext.RequestServices.GetRequiredService<UserManager<CuentaUsuario>>();
        var cuenta = await usuarios.FindByIdAsync(idCuenta.ToString());

        if (cuenta is null ||
            cuenta.SecurityStamp != selloToken ||
            cuenta.CuentaDeshabilitada ||
            cuenta.EstadoCodigo == EstadosCuentaCodigos.Inactiva)
        {
            contexto.Fail("La sesión ya no es válida. Inicia sesión de nuevo.");
            return;
        }

        // El rol se lee de la base en cada petición, así que un cambio de rol aplica sin esperar a que venza el token
        var esSuperAdmin = await usuarios.IsInRoleAsync(cuenta, RolesAplicacion.SuperAdmin);
        sesionBd.EstablecerUsuario(idCuenta, esSuperAdmin);
    }
}
