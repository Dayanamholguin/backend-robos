using AppRobos.Backend.Datos;
using AppRobos.Backend.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace AppRobos.Backend.Servicios;

// Borra cada hora los borradores de reporte cuyo plazo de 15 días ya venció.
// Corre sin usuario, así que se ejecuta en modo sistema para que la seguridad de filas no bloquee la limpieza.
public class BorradoresVencidosService : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromHours(1);

    private readonly IServiceScopeFactory _fabricaAmbitos;
    private readonly ILogger<BorradoresVencidosService> _registro;

    public BorradoresVencidosService(IServiceScopeFactory fabricaAmbitos, ILogger<BorradoresVencidosService> registro)
    {
        _fabricaAmbitos = fabricaAmbitos;
        _registro = registro;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var temporizador = new PeriodicTimer(Intervalo);
        while (await temporizador.WaitForNextTickAsync(ct))
        {
            await LimpiarAsync(ct);
        }
    }

    private async Task LimpiarAsync(CancellationToken ct)
    {
        try
        {
            using var ambito = _fabricaAmbitos.CreateScope();
            ambito.ServiceProvider.GetRequiredService<ContextoSesionBaseDatos>().ActivarModoSistema();

            var contexto = ambito.ServiceProvider.GetRequiredService<ContextoAplicacion>();
            var eliminados = await contexto.Database
                .SqlQuery<int>($"SELECT fn_eliminar_borradores_vencidos() AS \"Value\"")
                .SingleAsync(ct);

            if (eliminados > 0)
            {
                _registro.LogInformation("Se eliminaron {Cantidad} borradores vencidos.", eliminados);
            }
        }
        catch (Exception excepcion) when (excepcion is not OperationCanceledException)
        {
            _registro.LogWarning(excepcion, "No se pudo limpiar los borradores vencidos; se intentará de nuevo en la siguiente hora.");
        }
    }
}
