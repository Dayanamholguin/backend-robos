using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Modelos;
using Microsoft.Extensions.Options;

namespace AppRobos.Backend.Servicios;

// Resultado de registrar un intento fallido de login.
public enum ResultadoFalloLogin
{
    // Se registró el fallo, pero aún no se llega al límite de intentos.
    IntentoRegistrado,

    // Se llegó al límite y la cuenta quedó bloqueada por un tiempo.
    BloqueoTemporal,

    // Es el 4.º bloqueo: la cuenta queda deshabilitada de forma permanente.
    BloqueoPermanente
}

// Aplica el bloqueo escalonado de cuentas (RF-002):
// 3 intentos fallidos bloquean la cuenta 3 horas; el siguiente bloqueo dura 1 día;
// el tercero 1 semana; desde el cuarto la cuenta queda deshabilitada hasta contacto con soporte.
// Un login exitoso reinicia el escalonamiento.
public class ServicioBloqueoCuenta
{
    private readonly ReglasBloqueo _reglas;

    public ServicioBloqueoCuenta(IOptions<ReglasBloqueo> reglas)
    {
        _reglas = reglas.Value;
    }

    // Registra un intento fallido sobre la cuenta y decide si corresponde un bloqueo.
    // No guarda cambios en la base de datos; eso lo hace quien llama.
    // Cuenta cuyo login falló.
    // Momento actual en UTC.
    public ResultadoFalloLogin RegistrarFallo(CuentaUsuario cuenta, DateTimeOffset ahora)
    {
        // Cuenta un intento fallido más
        cuenta.AccessFailedCount++;

        // Si aún no llega al límite, solo queda registrado el intento
        if (cuenta.AccessFailedCount < _reglas.MaximoIntentosFallidos)
        {
            return ResultadoFalloLogin.IntentoRegistrado;
        }

        // Se llegó al límite: se cuenta un bloqueo nuevo y el contador de intentos vuelve a cero
        cuenta.AccessFailedCount = 0;
        cuenta.NivelBloqueo++;
        cuenta.EstadoCodigo = EstadosCuentaCodigos.Bloqueada;

        // Después del último bloqueo con duración definida, la cuenta queda deshabilitada
        if (cuenta.NivelBloqueo > _reglas.DuracionesHoras.Length)
        {
            cuenta.CuentaDeshabilitada = true;
            cuenta.LockoutEnd = null;
            return ResultadoFalloLogin.BloqueoPermanente;
        }

        // Bloqueo temporal: la duración depende del número de bloqueo (1.er bloqueo = primera posición)
        cuenta.LockoutEnd = ahora.AddHours(_reglas.DuracionesHoras[cuenta.NivelBloqueo - 1]);
        return ResultadoFalloLogin.BloqueoTemporal;
    }

    // Indica si la cuenta sigue dentro de un bloqueo temporal en este momento.
    public bool EstaBloqueadaTemporalmente(CuentaUsuario cuenta, DateTimeOffset ahora)
    {
        return cuenta.LockoutEnd.HasValue && cuenta.LockoutEnd.Value > ahora;
    }

    // Si el bloqueo temporal ya venció, devuelve la cuenta a estado ACTIVA.
    // Las cuentas deshabilitadas no se liberan aquí.
    public void LiberarBloqueoVencido(CuentaUsuario cuenta, DateTimeOffset ahora)
    {
        if (cuenta.CuentaDeshabilitada)
        {
            return;
        }

        if (cuenta.LockoutEnd.HasValue && cuenta.LockoutEnd.Value <= ahora)
        {
            cuenta.LockoutEnd = null;

            if (cuenta.EstadoCodigo == EstadosCuentaCodigos.Bloqueada)
            {
                cuenta.EstadoCodigo = EstadosCuentaCodigos.Activa;
            }
        }
    }

    // Reinicia el escalonamiento tras un login exitoso:
    // el contador de intentos y el nivel de bloqueo vuelven a cero.
    public void LimpiarTrasLoginExitoso(CuentaUsuario cuenta)
    {
        cuenta.AccessFailedCount = 0;
        cuenta.NivelBloqueo = 0;
        cuenta.LockoutEnd = null;

        if (cuenta.EstadoCodigo == EstadosCuentaCodigos.Bloqueada)
        {
            cuenta.EstadoCodigo = EstadosCuentaCodigos.Activa;
        }
    }
}
