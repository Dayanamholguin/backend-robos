using AppRobos.Backend.Dtos;
using AppRobos.Backend.Reportes;
using Xunit;

namespace BackEnd.Pruebas.Reportes;

// Pruebas de las reglas del reporte: franja horaria, limpieza de campos que no aplican,
// completitud de campos y validación del envío.
public class ReglasReportePruebas
{
    [Theory]
    [InlineData(0, "MADRUGADA")]
    [InlineData(5, "MADRUGADA")]
    [InlineData(6, "MANANA")]
    [InlineData(11, "MANANA")]
    [InlineData(12, "TARDE")]
    [InlineData(17, "TARDE")]
    [InlineData(18, "NOCHE")]
    [InlineData(23, "NOCHE")]
    public void FranjaSegunLaHora(int hora, string esperada)
    {
        Assert.Equal(esperada, MapeadorReporte.FranjaDeHora(new TimeOnly(hora, 30)));
    }

    [Fact]
    public void FranjaVaciaSinHora()
    {
        Assert.Null(MapeadorReporte.FranjaDeHora(null));
    }

    [Fact]
    public void TipoDeArmaSeBorraSiNoHuboArma()
    {
        var dto = DtoMinimo();
        dto.UsoArma = "NO";
        dto.TipoArma = "ARMA_FUEGO";
        var reporte = NuevoReporte();

        MapeadorReporte.Aplicar(dto, reporte, texto => texto);

        Assert.Null(reporte.TipoArma);
        Assert.Equal("NO", reporte.UsoArma);
    }

    [Fact]
    public void HoraSeBorraSiLaPrecisionNoEsExacta()
    {
        var dto = DtoMinimo();
        dto.PrecisionTemporal = "FRANJA_APROXIMADA";
        dto.HoraIncidente = new TimeOnly(20, 0);
        dto.FranjaHoraria = "NOCHE";
        var reporte = NuevoReporte();

        MapeadorReporte.Aplicar(dto, reporte, texto => texto);

        Assert.Null(reporte.HoraIncidente);
        Assert.Equal("NOCHE", reporte.FranjaHoraria);
    }

    [Fact]
    public void FranjaSeCalculaSolaConHoraExacta()
    {
        var dto = DtoMinimo();
        dto.PrecisionTemporal = "HORA_EXACTA";
        dto.HoraIncidente = new TimeOnly(19, 30);
        dto.FranjaHoraria = "MANANA";
        var reporte = NuevoReporte();

        MapeadorReporte.Aplicar(dto, reporte, texto => texto);

        Assert.Equal("NOCHE", reporte.FranjaHoraria);
    }

    [Fact]
    public void CedulaSeGuardaCifradaYNoEnTextoPlano()
    {
        var dto = DtoMinimo();
        dto.DatosPersonales = new DatosPersonalesEntradaDto
        {
            TipoParticipacion = "TESTIGO",
            ConsentimientoAceptado = true,
            NombreCompleto = "Persona Prueba",
            Cedula = "12345678",
            Correo = "persona@correo.com",
            Celular = "3001234567",
        };
        var reporte = NuevoReporte();

        MapeadorReporte.Aplicar(dto, reporte, texto => "cifrado:" + texto.Length);

        Assert.Equal("cifrado:8", reporte.DatosPersonales!.CedulaCifrada);
        Assert.DoesNotContain("12345678", reporte.DatosPersonales.CedulaCifrada);
    }

    [Fact]
    public void ValidacionDeEnvioPideCamposObligatorios()
    {
        var errores = ValidadorEnvioReporte.Validar(NuevoReporte(), cedulaRegistrada: false);

        Assert.Contains("Falta la fecha del incidente.", errores);
        Assert.Contains("Debes registrar al menos un bien sustraído.", errores);
        Assert.Contains("Faltan los datos de quien reporta.", errores);
    }

    [Fact]
    public void MomentoDelIncidenteSeConvierteAUtcConDesfaseDeColombia()
    {
        var reporte = NuevoReporte();
        reporte.PrecisionTemporal = "HORA_EXACTA";
        reporte.FechaIncidente = new DateOnly(2026, 10, 1);
        reporte.HoraIncidente = new TimeOnly(19, 0);

        var momento = MapeadorReporte.MomentoIncidenteUtc(reporte);

        Assert.Equal(new DateTimeOffset(2026, 10, 2, 0, 0, 0, TimeSpan.Zero), momento);
    }

    private static ReporteEntradaDto DtoMinimo() => new();

    private static Reporte NuevoReporte() => new()
    {
        Id = Guid.NewGuid(),
        UsuarioId = Guid.NewGuid(),
        EstadoReporte = EstadosReporte.Borrador,
    };
}

