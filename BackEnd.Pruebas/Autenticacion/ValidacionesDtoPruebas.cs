using System.ComponentModel.DataAnnotations;
using AppRobos.Backend.Dtos;
using Xunit;

namespace BackEnd.Pruebas.Autenticacion;

// Pruebas de las reglas de formato del registro: celular de Colombia, correo, contraseña,
// fecha de nacimiento y nombre.
public class ValidacionesDtoPruebas
{
    // Crea un registro válido; cada prueba cambia solo el campo que quiere probar.
    private static RegistroUsuarioDto RegistroValido(
        string nombre = "María",
        string apellido = "Pérez",
        string correo = "maria@correo.com",
        string contrasena = "Clave1234",
        string telefono = "3001234567",
        DateOnly? fechaNacimiento = null,
        string sexoCodigo = "FEMENINO") =>
        new()
        {
            Nombre = nombre,
            Apellido = apellido,
            Correo = correo,
            Contrasena = contrasena,
            Telefono = telefono,
            FechaNacimiento = fechaNacimiento ?? new DateOnly(1995, 6, 15),
            SexoCodigo = sexoCodigo
        };

    // Ejecuta las validaciones de DataAnnotations y devuelve los mensajes de error.
    private static List<ValidationResult> Validar(object dto)
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), resultados, validateAllProperties: true);
        return resultados;
    }

    [Fact]
    public void RegistroValidoNoTieneErrores()
    {
        Assert.Empty(Validar(RegistroValido()));
    }

    [Theory]
    [InlineData("3001234567", true)]
    [InlineData("3210123456", true)]
    [InlineData("3333333333", false)]  // cinco o más números iguales seguidos
    [InlineData("3111112222", false)]  // cinco o más números iguales seguidos
    [InlineData("2001234567", false)]   // no empieza por 3
    [InlineData("300123456", false)]    // 9 dígitos
    [InlineData("30012345678", false)]  // 11 dígitos
    [InlineData("300123456A", false)]   // contiene letras
    public void TelefonoDebeSerCelularDeColombia(string telefono, bool esValido)
    {
        var errores = Validar(RegistroValido(telefono: telefono));

        Assert.Equal(esValido, errores.Count == 0);
    }

    [Theory]
    [InlineData("correo-sin-arroba")]
    [InlineData("otro@")]
    public void CorreoInvalidoSeRechaza(string correo)
    {
        Assert.NotEmpty(Validar(RegistroValido(correo: correo)));
    }

    [Fact]
    public void ContrasenaDeMenosDeOchoCaracteresSeRechaza()
    {
        Assert.NotEmpty(Validar(RegistroValido(contrasena: "Ab1cdef")));
    }

    [Fact]
    public void FechaDeHoySeRechaza()
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        Assert.NotEmpty(Validar(RegistroValido(fechaNacimiento: hoy)));
    }

    [Fact]
    public void FechaAntesDe1900SeRechaza()
    {
        Assert.NotEmpty(Validar(RegistroValido(fechaNacimiento: new DateOnly(1899, 12, 31))));
    }

    [Theory]
    [InlineData("M4ria")]        // contiene números
    [InlineData("José-Ángel")]   // acentos y guion son válidos
    public void NombreSoloAceptaLetrasAcentosEspaciosApostrofesYGuiones(string nombre)
    {
        var esValido = nombre == "José-Ángel";

        Assert.Equal(esValido, Validar(RegistroValido(nombre: nombre)).Count == 0);
    }
}
