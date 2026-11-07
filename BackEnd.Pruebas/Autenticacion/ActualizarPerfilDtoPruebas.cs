using System.ComponentModel.DataAnnotations;
using AppRobos.Backend.Dtos;
using Xunit;

namespace BackEnd.Pruebas.Autenticacion;

// Pruebas de las reglas de la edición del perfil. Comparten las validaciones de los datos personales del registro.
public class ActualizarPerfilDtoPruebas
{
    // Crea un perfil válido; cada prueba cambia solo el campo que quiere probar.
    private static ActualizarPerfilDto PerfilValido() => new()
    {
        Nombre = "María",
        Apellido = "Pérez",
        Telefono = "3001234567",
        FechaNacimiento = new DateOnly(1995, 6, 15),
        SexoCodigo = "FEMENINO"
    };

    private static List<ValidationResult> Validar(object dto)
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), resultados, validateAllProperties: true);
        return resultados;
    }

    [Fact]
    public void PerfilValidoNoTieneErrores()
    {
        Assert.Empty(Validar(PerfilValido()));
    }

    [Fact]
    public void TelefonoInvalidoSeRechazaAlEditar()
    {
        var perfil = PerfilValido();
        perfil.Telefono = "2001234567";

        Assert.NotEmpty(Validar(perfil));
    }

    [Fact]
    public void FechaDeNacimientoFuturaSeRechazaAlEditar()
    {
        var perfil = PerfilValido();
        perfil.FechaNacimiento = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        Assert.NotEmpty(Validar(perfil));
    }

    [Fact]
    public void SexoVacioSeRechazaAlEditar()
    {
        var perfil = PerfilValido();
        perfil.SexoCodigo = string.Empty;

        Assert.NotEmpty(Validar(perfil));
    }

    [Fact]
    public void CambiarContrasenaRequiereLaActualYUnaNuevaDeOchoCaracteres()
    {
        var cambio = new CambiarContrasenaDto { ContrasenaActual = "Clave1234", ContrasenaNueva = "corta" };

        Assert.NotEmpty(Validar(cambio));
    }

    [Fact]
    public void CambiarContrasenaValidaNoTieneErrores()
    {
        var cambio = new CambiarContrasenaDto { ContrasenaActual = "Clave1234", ContrasenaNueva = "Nueva12345" };

        Assert.Empty(Validar(cambio));
    }
}
