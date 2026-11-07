using AppRobos.Backend.Icr;
using Xunit;

namespace BackEnd.Pruebas.Reportes;

// Pruebas del cálculo del ICR con los puntajes acordados.
public class CalculadoraIcrPruebas
{
    // Campos con dato suficientes para completitud 25: todos los relevantes y secundarios respondidos.
    private static List<CampoCompletitud> CamposCompletos() =>
    [
        new("Relevante1", true, true),
        new("Relevante2", true, true),
        new("Secundario1", false, true),
    ];

    private static EntradaIcr EntradaBase(
        IReadOnlyList<CampoCompletitud>? campos = null,
        FuenteUbicacion fuente = FuenteUbicacion.Gps,
        TimeSpan? tiempo = null,
        bool horaExacta = true,
        EstadoDenuncia denuncia = EstadoDenuncia.Si,
        bool radicado = false,
        bool consistencia = true) =>
        new(campos ?? CamposCompletos(), true, fuente, tiempo ?? TimeSpan.FromHours(2), horaExacta, denuncia, radicado, consistencia);

    [Fact]
    public void EjemploDelDocumentoDaOchentaYTresYAlta()
    {
        // Ejemplo del RF-074: completitud 25, ubicación manual 18, tiempo 30, denuncia 5 (no), radicado 0, consistencia 5
        var campos = new List<CampoCompletitud> { new("R1", true, true), new("S1", false, true) };
        var entrada = EntradaBase(campos, FuenteUbicacion.Manual, TimeSpan.FromHours(10), true, EstadoDenuncia.No, false, true);

        var resultado = CalculadoraIcr.Calcular(entrada);

        Assert.Equal(25, resultado.Puntajes[CalculadoraIcr.Completitud]);
        Assert.Equal(18, resultado.Puntajes[CalculadoraIcr.Ubicacion]);
        Assert.Equal(30, resultado.Puntajes[CalculadoraIcr.Tiempo]);
        Assert.Equal(5, resultado.Puntajes[CalculadoraIcr.Denuncia]);
        Assert.Equal(0, resultado.Puntajes[CalculadoraIcr.Radicado]);
        Assert.Equal(5, resultado.Puntajes[CalculadoraIcr.Consistencia]);
        Assert.Equal(83, resultado.Total);
        Assert.Equal(NivelIcr.Alta, resultado.Nivel);
    }

    [Theory]
    [InlineData(0, 0, 25)]   // sin campos sin dato
    [InlineData(0, 1, 20)]   // 1 secundario sin dato
    [InlineData(0, 3, 20)]   // 3 secundarios sin dato
    [InlineData(0, 4, 15)]   // 4 secundarios sin dato, ningún relevante
    [InlineData(1, 0, 15)]   // 1 relevante sin dato
    [InlineData(2, 0, 10)]   // 2 relevantes sin dato
    [InlineData(3, 0, 10)]   // 3 relevantes sin dato
    [InlineData(4, 0, 0)]    // 4 relevantes sin dato
    public void PuntosDeCompletitudSegunCamposSinDato(int relevantesSinDato, int secundariosSinDato, int esperado)
    {
        var campos = new List<CampoCompletitud>();
        for (var i = 0; i < 10; i++) campos.Add(new($"R{i}", true, i >= relevantesSinDato));
        for (var i = 0; i < 6; i++) campos.Add(new($"S{i}", false, i >= secundariosSinDato));

        Assert.Equal(esperado, CalculadoraIcr.PuntosCompletitud(EntradaBase(campos)));
    }

    [Fact]
    public void SinFechaOUbicacionValidaLaCompletitudEsCero()
    {
        var entrada = EntradaBase() with { FechaYUbicacionValidas = false };

        Assert.Equal(0, CalculadoraIcr.PuntosCompletitud(entrada));
    }

    [Theory]
    [InlineData(FuenteUbicacion.Gps, 20)]
    [InlineData(FuenteUbicacion.Manual, 18)]
    [InlineData(FuenteUbicacion.Otra, 0)]
    public void PuntosDeUbicacionSegunFuente(FuenteUbicacion fuente, int esperado)
    {
        Assert.Equal(esperado, CalculadoraIcr.PuntosUbicacion(fuente));
    }

    [Theory]
    [InlineData(1, 30)]
    [InlineData(48, 12)]
    [InlineData(100, 9)]
    [InlineData(500, 5)]
    [InlineData(800, 2)]
    public void PuntosDeTiempoPorHorasTranscurridas(int horas, int esperado)
    {
        var entrada = EntradaBase(tiempo: TimeSpan.FromHours(horas));

        Assert.Equal(esperado, CalculadoraIcr.PuntosTiempo(entrada));
    }

    [Fact]
    public void TiempoSinHoraExactaPuntuaCeroPorAhora()
    {
        var entrada = EntradaBase(tiempo: TimeSpan.FromHours(1), horaExacta: false);

        Assert.Equal(0, CalculadoraIcr.PuntosTiempo(entrada));
    }

    [Theory]
    [InlineData(EstadoDenuncia.Si, 10)]
    [InlineData(EstadoDenuncia.EnProceso, 10)]
    [InlineData(EstadoDenuncia.No, 5)]
    [InlineData(EstadoDenuncia.SinInformar, 0)]
    public void PuntosDeDenunciaSegunEstado(EstadoDenuncia estado, int esperado)
    {
        Assert.Equal(esperado, CalculadoraIcr.PuntosDenuncia(estado));
    }

    [Fact]
    public void RadicadoSumaSoloSiExiste()
    {
        var con = CalculadoraIcr.Calcular(EntradaBase(radicado: true));
        var sin = CalculadoraIcr.Calcular(EntradaBase(radicado: false));

        Assert.Equal(10, con.Puntajes[CalculadoraIcr.Radicado]);
        Assert.Equal(0, sin.Puntajes[CalculadoraIcr.Radicado]);
    }

    [Theory]
    [InlineData(0, NivelIcr.Baja)]
    [InlineData(39.99, NivelIcr.Baja)]
    [InlineData(40, NivelIcr.Media)]
    [InlineData(69.99, NivelIcr.Media)]
    [InlineData(70, NivelIcr.Alta)]
    [InlineData(89.99, NivelIcr.Alta)]
    [InlineData(90, NivelIcr.MuyAlta)]
    [InlineData(100, NivelIcr.MuyAlta)]
    public void ClasificacionPorRangos(double total, NivelIcr esperado)
    {
        Assert.Equal(esperado, CalculadoraIcr.Clasificar((decimal)total));
    }

    [Fact]
    public void PesosDeLaBaseEscalanLosPuntajes()
    {
        // Con los pesos por defecto el resultado es el mismo; con pesos distintos cambia proporcionalmente
        var pesos = new Dictionary<string, decimal>
        {
            [CalculadoraIcr.Completitud] = 50,
            [CalculadoraIcr.Ubicacion] = 20,
            [CalculadoraIcr.Tiempo] = 15,
            [CalculadoraIcr.Denuncia] = 10,
            [CalculadoraIcr.Radicado] = 10,
            [CalculadoraIcr.Consistencia] = 5,
        };

        var resultado = CalculadoraIcr.Calcular(EntradaBase(), pesos);

        Assert.Equal(50, resultado.Puntajes[CalculadoraIcr.Completitud]);
        Assert.Equal(15, resultado.Puntajes[CalculadoraIcr.Tiempo]);
    }
}
