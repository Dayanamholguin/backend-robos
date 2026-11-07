using AppRobos.Backend.Catalogos;
using AppRobos.Backend.Datos;
using AppRobos.Backend.Dtos;
using AppRobos.Backend.Icr;
using AppRobos.Backend.Modelos;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace AppRobos.Backend.Reportes;

// Casos de uso de los reportes del ciudadano: crear borrador, editarlo, borrarlo, listarlo y enviarlo.
// Todas las consultas pasan por la seguridad de filas: un ciudadano solo ve sus propios reportes.
public class ServicioReportes
{
    // Días que dura un borrador desde el primer guardado parcial.
    public const int DiasVigenciaBorrador = 15;

    private readonly ContextoAplicacion _contexto;
    private readonly ValidadorGuardadoReporte _validadorGuardado;
    private readonly ServicioOpcionesFormulario _opciones;
    private readonly IDataProtector _cifradoCedula;
    private readonly TimeProvider _reloj;

    public ServicioReportes(
        ContextoAplicacion contexto,
        ValidadorGuardadoReporte validadorGuardado,
        ServicioOpcionesFormulario opciones,
        IDataProtectionProvider proveedorCifrado,
        TimeProvider reloj)
    {
        _contexto = contexto;
        _validadorGuardado = validadorGuardado;
        _opciones = opciones;
        _cifradoCedula = proveedorCifrado.CreateProtector("ReporteDatosPersonales.Cedula");
        _reloj = reloj;
    }

    // Crea un borrador con los datos recibidos. Los campos obligatorios se revisan al enviar.
    public async Task<ResultadoReporte> CrearBorradorAsync(Guid usuarioId, ReporteEntradaDto dto, CancellationToken ct)
    {
        var errores = await _validadorGuardado.ValidarAsync(dto, ct);
        if (errores.Count > 0)
        {
            return ResultadoReporte.Invalido(errores);
        }

        var ahora = _reloj.GetUtcNow();
        var reporte = new Reporte
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            EstadoReporte = EstadosReporte.Borrador,
            FechaPrimerGuardado = ahora,
            FechaVencimientoBorrador = ahora.AddDays(DiasVigenciaBorrador),
            FechaActualizacion = ahora,
        };

        MapeadorReporte.Aplicar(dto, reporte, Cifrar);
        _contexto.Reportes.Add(reporte);
        await _contexto.SaveChangesAsync(ct);

        return ResultadoReporte.Exito(MapeadorRespuestaReporte.ADto(reporte));
    }

    // Reemplaza los datos de un borrador. El plazo de vencimiento no cambia.
    public async Task<ResultadoReporte> ActualizarBorradorAsync(Guid usuarioId, Guid reporteId, ReporteEntradaDto dto, CancellationToken ct)
    {
        // Solo el autor puede modificar su reporte; a los demás se responde como si no existiera
        var reporte = await _contexto.Reportes.FirstOrDefaultAsync(r => r.Id == reporteId && r.UsuarioId == usuarioId, ct);
        if (reporte is null)
        {
            return ResultadoReporte.NoEncontrado();
        }
        if (reporte.EstadoReporte == EstadosReporte.Enviado)
        {
            return ResultadoReporte.Conflicto("El reporte ya fue enviado y no se puede modificar.");
        }

        var errores = await _validadorGuardado.ValidarAsync(dto, ct);
        if (errores.Count > 0)
        {
            return ResultadoReporte.Invalido(errores);
        }

        // La cédula cifrada se conserva si la app no la vuelve a enviar
        var cedulaActual = await _contexto.Set<ReporteDatosPersonales>()
            .AsNoTracking()
            .Where(d => d.ReporteId == reporteId)
            .Select(d => d.CedulaCifrada)
            .FirstOrDefaultAsync(ct);
        reporte.DatosPersonales = cedulaActual is null ? null : new ReporteDatosPersonales { CedulaCifrada = cedulaActual };

        await using var transaccion = await _contexto.Database.BeginTransactionAsync(ct);

        // Se borran las filas hijas y se insertan las nuevas
        await _contexto.Set<ReporteBien>().Where(b => b.ReporteId == reporteId).ExecuteDeleteAsync(ct);
        await _contexto.Set<ReporteCaracteristicaSector>().Where(c => c.ReporteId == reporteId).ExecuteDeleteAsync(ct);
        await _contexto.Set<ReporteAccesoForzado>().Where(a => a.ReporteId == reporteId).ExecuteDeleteAsync(ct);
        await _contexto.Set<ReporteResponsables>().Where(r => r.ReporteId == reporteId).ExecuteDeleteAsync(ct);
        await _contexto.Set<ReporteDatosPersonales>().Where(d => d.ReporteId == reporteId).ExecuteDeleteAsync(ct);

        MapeadorReporte.Aplicar(dto, reporte, Cifrar);
        reporte.FechaActualizacion = _reloj.GetUtcNow();
        await _contexto.SaveChangesAsync(ct);
        await transaccion.CommitAsync(ct);

        var completo = await ObtenerCompletoAsync(reporteId, ct);
        return ResultadoReporte.Exito(MapeadorRespuestaReporte.ADto(completo!));
    }

    // Devuelve un reporte con todas sus secciones, si pertenece al ciudadano.
    public async Task<ResultadoReporte> ObtenerAsync(Guid reporteId, CancellationToken ct)
    {
        var reporte = await ObtenerCompletoAsync(reporteId, ct);
        return reporte is null
            ? ResultadoReporte.NoEncontrado()
            : ResultadoReporte.Exito(MapeadorRespuestaReporte.ADto(reporte));
    }

    // Lista los reportes del ciudadano, los más recientes primero.
    public async Task<IReadOnlyList<ReporteResumenDto>> ListarAsync(CancellationToken ct)
    {
        var reportes = await _contexto.Reportes
            .AsNoTracking()
            .OrderByDescending(r => r.FechaActualizacion)
            .Take(100)
            .ToListAsync(ct);

        return reportes.Select(MapeadorRespuestaReporte.AResumen).ToList();
    }

    // Borra un borrador. Un reporte enviado no se puede borrar.
    public async Task<ResultadoReporte> EliminarBorradorAsync(Guid usuarioId, Guid reporteId, CancellationToken ct)
    {
        // Solo el autor puede borrar su reporte
        var reporte = await _contexto.Reportes.FirstOrDefaultAsync(r => r.Id == reporteId && r.UsuarioId == usuarioId, ct);
        if (reporte is null)
        {
            return ResultadoReporte.NoEncontrado();
        }
        if (reporte.EstadoReporte == EstadosReporte.Enviado)
        {
            return ResultadoReporte.Conflicto("Un reporte enviado no se puede borrar.");
        }

        _contexto.Reportes.Remove(reporte);
        await _contexto.SaveChangesAsync(ct);
        return ResultadoReporte.SinContenido();
    }

    // Envía el borrador: revisa los campos obligatorios, calcula el ICR y deja el reporte inmutable.
    public async Task<ResultadoReporte> EnviarAsync(Guid usuarioId, Guid reporteId, CancellationToken ct)
    {
        // Solo el autor puede enviar su reporte; el superadmin, aunque lo vea, no puede hacerlo
        if (!await _contexto.Reportes.AnyAsync(r => r.Id == reporteId && r.UsuarioId == usuarioId, ct))
        {
            return ResultadoReporte.NoEncontrado();
        }

        var reporte = await ObtenerCompletoAsync(reporteId, ct, seguimiento: true);
        if (reporte is null)
        {
            return ResultadoReporte.NoEncontrado();
        }
        if (reporte.EstadoReporte == EstadosReporte.Enviado)
        {
            return ResultadoReporte.Conflicto("El reporte ya fue enviado.");
        }

        var cedulaRegistrada = !string.IsNullOrEmpty(reporte.DatosPersonales?.CedulaCifrada);
        var errores = ValidadorEnvioReporte.Validar(reporte, cedulaRegistrada);
        if (errores.Count > 0)
        {
            return ResultadoReporte.Invalido(errores);
        }

        var ahora = _reloj.GetUtcNow();
        var icr = await CalcularIcrAsync(reporte, ahora, ct);

        reporte.EstadoReporte = EstadosReporte.Enviado;
        reporte.FechaEnvio = ahora;
        reporte.FechaVencimientoBorrador = null;
        reporte.FechaActualizacion = ahora;
        reporte.IcrPuntaje = icr.Total;
        reporte.IcrClasificacion = CodigoNivel(icr.Nivel);
        foreach (var (criterio, puntaje) in icr.Puntajes)
        {
            reporte.DetalleIcr.Add(new ReporteIcr { ReporteId = reporte.Id, Criterio = criterio, Puntaje = puntaje });
        }

        await _contexto.SaveChangesAsync(ct);

        return ResultadoReporte.Exito(MapeadorRespuestaReporte.ADto(reporte));
    }

    // Arma los datos del ICR a partir del reporte y calcula el puntaje.
    private async Task<ResultadoIcr> CalcularIcrAsync(Reporte reporte, DateTimeOffset ahora, CancellationToken ct)
    {
        var opciones = await _opciones.CargarAsync(ct);
        var campos = ConstructorCamposCompletitud.Generar(reporte, opciones.EsDato);

        var pesos = await _contexto.Set<CriterioIcr>()
            .AsNoTracking()
            .Where(c => c.Activo)
            .ToDictionaryAsync(c => c.Codigo, c => c.Peso, ct);

        var momento = MapeadorReporte.MomentoIncidenteUtc(reporte);
        var tiempo = momento is { } m && ahora >= m ? ahora - m : (TimeSpan?)null;

        var entrada = new EntradaIcr(
            Campos: campos,
            FechaYUbicacionValidas: true,
            Fuente: reporte.TipoUbicacion switch
            {
                "GPS_ACTUAL" => FuenteUbicacion.Gps,
                "PUNTO_MAPA" => FuenteUbicacion.Manual,
                _ => FuenteUbicacion.Otra,
            },
            TiempoTranscurrido: tiempo,
            TiempoEsHoraExacta: reporte.PrecisionTemporal == "HORA_EXACTA",
            Denuncia: reporte.InformoAutoridad switch
            {
                "SI" => EstadoDenuncia.Si,
                "EN_PROCESO" => EstadoDenuncia.EnProceso,
                "NO" => EstadoDenuncia.No,
                _ => EstadoDenuncia.SinInformar,
            },
            TieneRadicado: reporte.TieneRadicado == "SI" && !string.IsNullOrWhiteSpace(reporte.NumeroRadicado),
            ConsistenciaOk: true);

        return CalculadoraIcr.Calcular(entrada, pesos);
    }

    // Carga el reporte con todas sus secciones. Con seguimiento=true sirve para modificarlo.
    private async Task<Reporte?> ObtenerCompletoAsync(Guid reporteId, CancellationToken ct, bool seguimiento = false)
    {
        var consulta = _contexto.Reportes
            .Include(r => r.Bienes).ThenInclude(b => b.Celular)
            .Include(r => r.Bienes).ThenInclude(b => b.Vehiculo)
            .Include(r => r.Bienes).ThenInclude(b => b.Tarjeta)
            .Include(r => r.Bienes).ThenInclude(b => b.Documento)
            .Include(r => r.Bienes).ThenInclude(b => b.ContenidoBolso)
            .Include(r => r.CaracteristicasSector)
            .Include(r => r.AccesosForzados)
            .Include(r => r.Responsables)
            .Include(r => r.DatosPersonales)
            .Include(r => r.DetalleIcr)
            .AsSplitQuery()
            .Where(r => r.Id == reporteId);

        if (!seguimiento)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(ct);
    }

    private string Cifrar(string texto) => _cifradoCedula.Protect(texto);

    private static string CodigoNivel(NivelIcr nivel) => nivel switch
    {
        NivelIcr.Baja => "BAJA",
        NivelIcr.Media => "MEDIA",
        NivelIcr.Alta => "ALTA",
        _ => "MUY_ALTA",
    };
}

// Resultado de una operación sobre reportes. El controlador lo traduce a HTTP.
public sealed record ResultadoReporte(TipoResultadoReporte Tipo, ReporteRespuestaDto? Datos, IReadOnlyList<string> Mensajes)
{
    public static ResultadoReporte Exito(ReporteRespuestaDto datos) => new(TipoResultadoReporte.Exito, datos, []);
    public static ResultadoReporte SinContenido() => new(TipoResultadoReporte.Exito, null, []);
    public static ResultadoReporte Invalido(IEnumerable<string> mensajes) => new(TipoResultadoReporte.Invalido, null, mensajes.ToList());
    public static ResultadoReporte Conflicto(string mensaje) => new(TipoResultadoReporte.Conflicto, null, [mensaje]);
    public static ResultadoReporte NoEncontrado() => new(TipoResultadoReporte.NoEncontrado, null, []);
}

public enum TipoResultadoReporte
{
    Exito,
    Invalido,
    Conflicto,
    NoEncontrado,
}
