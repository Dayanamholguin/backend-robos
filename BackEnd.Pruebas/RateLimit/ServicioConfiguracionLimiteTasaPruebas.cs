using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Modelos;
using AppRobos.Backend.Servicios;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BackEnd.Pruebas.RateLimit;

// Pruebas de la copia en memoria de las políticas: validación, respaldo ante valores inválidos
// y versionado al cambiar un límite.
public class ServicioConfiguracionLimiteTasaPruebas
{
    // Crea el servicio sin base de datos: AplicarPoliticas no necesita el ámbito de EF.
    private static ServicioConfiguracionLimiteTasa CrearServicio() =>
        new(null!, NullLogger<ServicioConfiguracionLimiteTasa>.Instance);

    // Crea una política válida con los valores indicados.
    private static PoliticaLimiteTasa Politica(string nombre, int limite, int ventana = 60, bool activa = true) => new()
    {
        Nombre = nombre,
        LimitePeticiones = limite,
        VentanaSegundos = ventana,
        EstrategiaParticion = "IP",
        LimiteCola = 0,
        Activa = activa
    };

    // Conjunto completo de políticas, con los valores iniciales de la migración.
    private static List<PoliticaLimiteTasa> ConjuntoInicial() =>
    [
        Politica(NombresPoliticasLimiteTasa.Login, 5),
        Politica(NombresPoliticasLimiteTasa.Registro, 3),
        Politica(NombresPoliticasLimiteTasa.LecturaReportes, 60),
        Politica(NombresPoliticasLimiteTasa.PorDefecto, 100)
    ];

    [Fact]
    public void PoliticaValidaNoTieneError()
    {
        Assert.Null(ServicioConfiguracionLimiteTasa.ValidarPolitica(Politica(NombresPoliticasLimiteTasa.Login, 5)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10001)]
    public void LimiteFueraDeRangoSeRechaza(int limite)
    {
        Assert.NotNull(ServicioConfiguracionLimiteTasa.ValidarPolitica(Politica(NombresPoliticasLimiteTasa.Login, limite)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3601)]
    public void VentanaFueraDeRangoSeRechaza(int ventana)
    {
        Assert.NotNull(ServicioConfiguracionLimiteTasa.ValidarPolitica(Politica(NombresPoliticasLimiteTasa.Login, 5, ventana)));
    }

    [Fact]
    public void ColaDistintaDeCeroSeRechaza()
    {
        var politica = Politica(NombresPoliticasLimiteTasa.Login, 5);
        politica.LimiteCola = 2;

        Assert.NotNull(ServicioConfiguracionLimiteTasa.ValidarPolitica(politica));
    }

    [Fact]
    public void LoginNoSePuedeDesactivar()
    {
        Assert.NotNull(ServicioConfiguracionLimiteTasa.ValidarPolitica(Politica(NombresPoliticasLimiteTasa.Login, 5, activa: false)));
        Assert.NotNull(ServicioConfiguracionLimiteTasa.ValidarPolitica(Politica(NombresPoliticasLimiteTasa.Registro, 3, activa: false)));
    }

    [Fact]
    public void PoliticaPorDefectoSiPuedeDesactivarse()
    {
        Assert.Null(ServicioConfiguracionLimiteTasa.ValidarPolitica(Politica(NombresPoliticasLimiteTasa.PorDefecto, 100, activa: false)));
    }

    [Fact]
    public void NombreDesconocidoSeRechaza()
    {
        Assert.NotNull(ServicioConfiguracionLimiteTasa.ValidarPolitica(Politica("PoliticaInventada", 5)));
    }

    [Fact]
    public void AplicarMismosValoresNoCreaNuevaVersion()
    {
        var servicio = CrearServicio();
        servicio.AplicarPoliticas(ConjuntoInicial());
        var versionInicial = servicio.Actual.Version;

        var hubo = servicio.AplicarPoliticas(ConjuntoInicial());

        Assert.False(hubo);
        Assert.Equal(versionInicial, servicio.Actual.Version);
    }

    [Fact]
    public void CambiarUnLimiteCreaNuevaVersionYSeAplica()
    {
        var servicio = CrearServicio();
        servicio.AplicarPoliticas(ConjuntoInicial());
        var versionInicial = servicio.Actual.Version;
        var cambiado = ConjuntoInicial();
        cambiado[0].LimitePeticiones = 10;

        var hubo = servicio.AplicarPoliticas(cambiado);

        Assert.True(hubo);
        Assert.True(servicio.Actual.Version > versionInicial);
        Assert.Equal(10, servicio.Actual.Obtener(NombresPoliticasLimiteTasa.Login)!.LimitePeticiones);
    }

    [Fact]
    public void PoliticaInvalidaConservaLaUltimaValida()
    {
        var servicio = CrearServicio();
        servicio.AplicarPoliticas(ConjuntoInicial());
        var invalido = ConjuntoInicial();
        invalido[0].LimitePeticiones = 0;

        servicio.AplicarPoliticas(invalido);

        Assert.Equal(5, servicio.Actual.Obtener(NombresPoliticasLimiteTasa.Login)!.LimitePeticiones);
    }

    [Fact]
    public void LoginDesactivadoEnLaBdSeIgnoraYConservaLaValida()
    {
        var servicio = CrearServicio();
        servicio.AplicarPoliticas(ConjuntoInicial());
        var sinLogin = ConjuntoInicial();
        sinLogin[0].Activa = false;

        servicio.AplicarPoliticas(sinLogin);

        Assert.True(servicio.Actual.Obtener(NombresPoliticasLimiteTasa.Login)!.Activa);
    }

    [Fact]
    public void SinConfiguracionEnBdSeUsanLosValoresDeRespaldo()
    {
        var servicio = CrearServicio();

        Assert.Equal(5, servicio.Actual.Obtener(NombresPoliticasLimiteTasa.Login)!.LimitePeticiones);
        Assert.Equal(3, servicio.Actual.Obtener(NombresPoliticasLimiteTasa.Registro)!.LimitePeticiones);
        Assert.Equal(100, servicio.Actual.Obtener(NombresPoliticasLimiteTasa.PorDefecto)!.LimitePeticiones);
    }
}
