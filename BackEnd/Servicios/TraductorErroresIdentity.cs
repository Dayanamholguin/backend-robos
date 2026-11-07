using Microsoft.AspNetCore.Identity;

namespace AppRobos.Backend.Servicios;

// Traduce los errores de Identity (en inglés) a mensajes en español para el ciudadano.
// Usa el código del error, no el texto, para que la traducción no dependa del idioma del sistema.
public static class TraductorErroresIdentity
{
    private static readonly IReadOnlyDictionary<string, string> Mensajes = new Dictionary<string, string>
    {
        ["PasswordTooShort"] = "La contraseña debe tener al menos 8 caracteres.",
        ["PasswordRequiresUpper"] = "La contraseña debe tener al menos una letra mayúscula.",
        ["PasswordRequiresLower"] = "La contraseña debe tener al menos una letra minúscula.",
        ["PasswordRequiresDigit"] = "La contraseña debe tener al menos un número.",
        ["PasswordRequiresNonAlphanumeric"] = "La contraseña debe tener al menos un carácter especial.",
        ["PasswordRequiresUniqueChars"] = "La contraseña debe tener caracteres distintos.",
        ["PasswordMismatch"] = "La contraseña actual no es correcta.",
        ["DuplicateEmail"] = "Ya existe una cuenta con ese correo electrónico.",
        ["DuplicateUserName"] = "Ya existe una cuenta con ese correo electrónico.",
        ["InvalidEmail"] = "El correo electrónico no tiene un formato válido.",
        ["InvalidUserName"] = "El correo electrónico no tiene un formato válido.",
        ["ConcurrencyFailure"] = "La cuenta fue modificada al mismo tiempo. Vuelve a intentarlo.",
    };

    // Devuelve el mensaje en español para cada error. Si el código no está en la lista,
    // se usa un mensaje genérico para no mostrar texto en inglés.
    public static IReadOnlyList<string> Traducir(IEnumerable<IdentityError> errores) =>
        errores
            .Select(e => Mensajes.TryGetValue(e.Code, out var mensaje)
                ? mensaje
                : "No se pudo completar la operación con los datos enviados.")
            .Distinct()
            .ToList();
}
