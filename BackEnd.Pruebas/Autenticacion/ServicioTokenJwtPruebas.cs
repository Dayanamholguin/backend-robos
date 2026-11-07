using System.IdentityModel.Tokens.Jwt;
using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Modelos;
using AppRobos.Backend.Servicios;
using Microsoft.Extensions.Options;
using Xunit;

namespace BackEnd.Pruebas.Autenticacion;

// Pruebas del token JWT: claims, emisor, audiencia y duración configurada.
public class ServicioTokenJwtPruebas
{
    private static readonly DateTimeOffset Ahora = new(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);

    // Configuración de prueba. La clave es de prueba y nunca se usa fuera de las pruebas.
    private static ServicioTokenJwt CrearServicio(int minutos = 60) =>
        new(Options.Create(new ConfiguracionJwt
        {
            Clave = "clave-de-prueba-solo-para-pruebas-unitarias-32",
            Emisor = "AppRobos.Backend",
            Audiencia = "AppRobos.App",
            MinutosExpiracion = minutos
        }));

    [Fact]
    public void TokenIncluyeIdentificadorCorreoYRol()
    {
        var cuenta = new CuentaUsuario { Id = Guid.NewGuid(), Email = "maria@correo.com" };

        var token = CrearServicio().CrearToken(cuenta, [RolesAplicacion.Ciudadano], Ahora);
        var leido = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);

        Assert.Equal(cuenta.Id.ToString(), leido.Subject);
        Assert.Equal("maria@correo.com", leido.Claims.First(c => c.Type == "email").Value);
        Assert.Equal(RolesAplicacion.Ciudadano, leido.Claims.First(c => c.Type == "role").Value);
    }

    [Fact]
    public void TokenVenceSegunLosMinutosConfigurados()
    {
        var cuenta = new CuentaUsuario { Id = Guid.NewGuid(), Email = "maria@correo.com" };

        var token = CrearServicio(minutos: 60).CrearToken(cuenta, [RolesAplicacion.Ciudadano], Ahora);

        Assert.Equal(Ahora.AddMinutes(60), token.ExpiraEn);
    }

    [Fact]
    public void TokenTieneEmisorYAudienciaConfigurados()
    {
        var cuenta = new CuentaUsuario { Id = Guid.NewGuid(), Email = "maria@correo.com" };

        var token = CrearServicio().CrearToken(cuenta, [RolesAplicacion.Ciudadano], Ahora);
        var leido = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);

        Assert.Equal("AppRobos.Backend", leido.Issuer);
        Assert.Contains("AppRobos.App", leido.Audiences);
    }
}
