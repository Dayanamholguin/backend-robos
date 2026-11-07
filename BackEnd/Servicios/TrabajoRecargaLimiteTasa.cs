namespace AppRobos.Backend.Servicios;

// Proceso en segundo plano que recarga las políticas de rate limiting desde la BD.
// Carga la configuración al arrancar y después cada 60 segundos.
public class TrabajoRecargaLimiteTasa : BackgroundService
{
    private readonly ServicioConfiguracionLimiteTasa _configuracion;

    public TrabajoRecargaLimiteTasa(ServicioConfiguracionLimiteTasa configuracion)
    {
        _configuracion = configuracion;
    }

    // Repite la recarga hasta que la aplicación se detenga.
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Primera carga al arrancar; si falla, se usan los valores de respaldo
        await _configuracion.RecargarAsync(ct);

        using var temporizador = new PeriodicTimer(ServicioConfiguracionLimiteTasa.IntervaloRecarga);
        while (await temporizador.WaitForNextTickAsync(ct))
        {
            await _configuracion.RecargarAsync(ct);
        }
    }
}
