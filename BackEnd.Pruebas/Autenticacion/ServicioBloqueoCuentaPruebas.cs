using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Modelos;
using AppRobos.Backend.Servicios;
using Microsoft.Extensions.Options;
using Xunit;

namespace BackEnd.Pruebas.Autenticacion;

// Pruebas del bloqueo escalonado: 3 fallos bloquean 3 h, luego 1 día, luego 1 semana,
// y desde el 4.º bloqueo la cuenta queda deshabilitada. Un login exitoso reinicia el escalonamiento.
public class ServicioBloqueoCuentaPruebas
{
    private static readonly DateTimeOffset Ahora = new(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);

    // Crea el servicio con las reglas por defecto: 3 intentos y bloqueos de 3 h, 24 h y 168 h.
    private static ServicioBloqueoCuenta CrearServicio() =>
        new(Options.Create(new ReglasBloqueo()));

    // Crea una cuenta limpia en estado activo.
    private static CuentaUsuario CrearCuenta() =>
        new() { EstadoCodigo = EstadosCuentaCodigos.Activa };

    // Registra la cantidad de fallos indicada y devuelve el último resultado.
    private static ResultadoFalloLogin RegistrarFallos(ServicioBloqueoCuenta servicio, CuentaUsuario cuenta, int cantidad, DateTimeOffset ahora)
    {
        var resultado = ResultadoFalloLogin.IntentoRegistrado;
        for (var i = 0; i < cantidad; i++)
        {
            resultado = servicio.RegistrarFallo(cuenta, ahora);
        }
        return resultado;
    }

    [Fact]
    public void DosFallosNoBloquean()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();

        var resultado = RegistrarFallos(servicio, cuenta, 2, Ahora);

        Assert.Equal(ResultadoFalloLogin.IntentoRegistrado, resultado);
        Assert.Equal(EstadosCuentaCodigos.Activa, cuenta.EstadoCodigo);
        Assert.Null(cuenta.LockoutEnd);
    }

    [Fact]
    public void TercerFalloBloqueaTresHoras()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();

        var resultado = RegistrarFallos(servicio, cuenta, 3, Ahora);

        Assert.Equal(ResultadoFalloLogin.BloqueoTemporal, resultado);
        Assert.Equal(EstadosCuentaCodigos.Bloqueada, cuenta.EstadoCodigo);
        Assert.Equal(1, cuenta.NivelBloqueo);
        Assert.Equal(Ahora.AddHours(3), cuenta.LockoutEnd);
        Assert.False(cuenta.CuentaDeshabilitada);
    }

    [Fact]
    public void SegundoBloqueoDuraUnDia()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();
        RegistrarFallos(servicio, cuenta, 3, Ahora);

        // Al vencer el primer bloqueo, la cuenta vuelve a activa y se vuelven a fallar 3 veces
        var despues = Ahora.AddHours(3);
        servicio.LiberarBloqueoVencido(cuenta, despues);
        var resultado = RegistrarFallos(servicio, cuenta, 3, despues);

        Assert.Equal(ResultadoFalloLogin.BloqueoTemporal, resultado);
        Assert.Equal(2, cuenta.NivelBloqueo);
        Assert.Equal(despues.AddHours(24), cuenta.LockoutEnd);
    }

    [Fact]
    public void TercerBloqueoDuraUnaSemana()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();
        cuenta.NivelBloqueo = 2;

        var resultado = RegistrarFallos(servicio, cuenta, 3, Ahora);

        Assert.Equal(ResultadoFalloLogin.BloqueoTemporal, resultado);
        Assert.Equal(3, cuenta.NivelBloqueo);
        Assert.Equal(Ahora.AddHours(168), cuenta.LockoutEnd);
    }

    [Fact]
    public void CuartoBloqueoDeshabilitaLaCuentaDeFormaPermanente()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();
        cuenta.NivelBloqueo = 3;

        var resultado = RegistrarFallos(servicio, cuenta, 3, Ahora);

        Assert.Equal(ResultadoFalloLogin.BloqueoPermanente, resultado);
        Assert.True(cuenta.CuentaDeshabilitada);
        Assert.Equal(EstadosCuentaCodigos.Bloqueada, cuenta.EstadoCodigo);
        Assert.Null(cuenta.LockoutEnd);
    }

    [Fact]
    public void LiberarBloqueoVencidoActivaLaCuentaCuandoPasaElPlazo()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();
        RegistrarFallos(servicio, cuenta, 3, Ahora);

        servicio.LiberarBloqueoVencido(cuenta, Ahora.AddHours(3).AddSeconds(1));

        Assert.Equal(EstadosCuentaCodigos.Activa, cuenta.EstadoCodigo);
        Assert.Null(cuenta.LockoutEnd);
    }

    [Fact]
    public void LiberarBloqueoNoActivaUnaCuentaDeshabilitada()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();
        cuenta.NivelBloqueo = 3;
        RegistrarFallos(servicio, cuenta, 3, Ahora);

        servicio.LiberarBloqueoVencido(cuenta, Ahora.AddYears(10));

        Assert.True(cuenta.CuentaDeshabilitada);
        Assert.Equal(EstadosCuentaCodigos.Bloqueada, cuenta.EstadoCodigo);
    }

    [Fact]
    public void LoginExitosoReiniciaElEscalonamiento()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();
        RegistrarFallos(servicio, cuenta, 3, Ahora);
        servicio.LiberarBloqueoVencido(cuenta, Ahora.AddHours(4));
        RegistrarFallos(servicio, cuenta, 2, Ahora.AddHours(4));

        servicio.LimpiarTrasLoginExitoso(cuenta);

        Assert.Equal(0, cuenta.NivelBloqueo);
        Assert.Equal(0, cuenta.AccessFailedCount);
        Assert.Null(cuenta.LockoutEnd);
        Assert.Equal(EstadosCuentaCodigos.Activa, cuenta.EstadoCodigo);
    }

    [Fact]
    public void TrasLoginExitosoElSiguienteBloqueoVuelveADurarTresHoras()
    {
        var servicio = CrearServicio();
        var cuenta = CrearCuenta();
        RegistrarFallos(servicio, cuenta, 3, Ahora);
        servicio.LiberarBloqueoVencido(cuenta, Ahora.AddHours(4));
        servicio.LimpiarTrasLoginExitoso(cuenta);

        var resultado = RegistrarFallos(servicio, cuenta, 3, Ahora.AddHours(5));

        Assert.Equal(ResultadoFalloLogin.BloqueoTemporal, resultado);
        Assert.Equal(1, cuenta.NivelBloqueo);
        Assert.Equal(Ahora.AddHours(8), cuenta.LockoutEnd);
    }
}
