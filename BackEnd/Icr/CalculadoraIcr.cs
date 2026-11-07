namespace AppRobos.Backend.Icr;

// Calcula el Índice de Confiabilidad del Reporte (0 a 100) a partir de datos ya leídos.
// No consulta la base de datos: así se puede probar sin infraestructura.
public static class CalculadoraIcr
{
    // Códigos de los criterios. Coinciden con la tabla CriteriosIcr.
    public const string Completitud = "COMPLETITUD";
    public const string Ubicacion = "UBICACION";
    public const string Tiempo = "TIEMPO";
    public const string Denuncia = "DENUNCIA";
    public const string Radicado = "RADICADO";
    public const string Consistencia = "CONSISTENCIA";

    // Puntaje máximo de cada criterio. El peso de la base se aplica proporcionalmente sobre este máximo.
    private static readonly IReadOnlyDictionary<string, decimal> PuntajeMaximo = new Dictionary<string, decimal>
    {
        [Completitud] = 25,
        [Ubicacion] = 20,
        [Tiempo] = 30,
        [Denuncia] = 10,
        [Radicado] = 10,
        [Consistencia] = 5,
    };

    // Pesos de respaldo si la base no tiene un criterio. Son los valores acordados.
    private static readonly IReadOnlyDictionary<string, decimal> PesosPorDefecto = PuntajeMaximo;

    public static ResultadoIcr Calcular(EntradaIcr entrada, IReadOnlyDictionary<string, decimal>? pesos = null)
    {
        var pesoDe = new Func<string, decimal>(codigo =>
            pesos is not null && pesos.TryGetValue(codigo, out var peso) ? peso : PesosPorDefecto[codigo]);

        var puntajes = new Dictionary<string, decimal>
        {
            [Completitud] = Escalar(PuntosCompletitud(entrada), Completitud, pesoDe),
            [Ubicacion] = Escalar(PuntosUbicacion(entrada.Fuente), Ubicacion, pesoDe),
            [Tiempo] = Escalar(PuntosTiempo(entrada), Tiempo, pesoDe),
            [Denuncia] = Escalar(PuntosDenuncia(entrada.Denuncia), Denuncia, pesoDe),
            [Radicado] = Escalar(entrada.TieneRadicado ? PuntajeMaximo[Radicado] : 0, Radicado, pesoDe),
            [Consistencia] = Escalar(entrada.ConsistenciaOk ? PuntajeMaximo[Consistencia] : 0, Consistencia, pesoDe),
        };

        var total = puntajes.Values.Sum();
        return new ResultadoIcr(total, Clasificar(total), puntajes);
    }

    // Completitud: 25, 20, 15, 10 o 0 según los campos relevantes y secundarios con dato.
    // Sin fecha o ubicación válidas el puntaje es 0.
    public static decimal PuntosCompletitud(EntradaIcr entrada)
    {
        if (!entrada.FechaYUbicacionValidas)
        {
            return 0;
        }

        var relevantesSinDato = entrada.Campos.Count(c => c.EsRelevante && !c.TieneDato);
        var secundariosSinDato = entrada.Campos.Count(c => !c.EsRelevante && !c.TieneDato);

        return relevantesSinDato switch
        {
            0 when secundariosSinDato == 0 => 25,
            0 when secundariosSinDato <= 3 => 20,
            0 => 15,
            1 => 15,
            <= 3 => 10,
            _ => 0,
        };
    }

    // Ubicación: GPS 20, selección manual 18, cualquier otra fuente 0.
    public static decimal PuntosUbicacion(FuenteUbicacion fuente) => fuente switch
    {
        FuenteUbicacion.Gps => 20,
        FuenteUbicacion.Manual => 18,
        _ => 0,
    };

    // Tiempo transcurrido entre el incidente y el envío.
    // Solo la hora exacta puntúa; la franja aproximada y el momento desconocido valen 0.
    public static decimal PuntosTiempo(EntradaIcr entrada)
    {
        if (entrada.TiempoTranscurrido is not { } tiempo || tiempo < TimeSpan.Zero || !entrada.TiempoEsHoraExacta)
        {
            return 0;
        }

        if (tiempo <= TimeSpan.FromHours(24)) return 30;
        if (tiempo <= TimeSpan.FromDays(3)) return 12;
        if (tiempo <= TimeSpan.FromDays(7)) return 9;
        if (tiempo <= TimeSpan.FromDays(30)) return 5;
        return 2;
    }

    // Denuncia: sí o en proceso 10, no 5, sin información 0.
    public static decimal PuntosDenuncia(EstadoDenuncia estado) => estado switch
    {
        EstadoDenuncia.Si => 10,
        EstadoDenuncia.EnProceso => 10,
        EstadoDenuncia.No => 5,
        _ => 0,
    };

    // Clasificación del puntaje total: Baja, Media, Alta o Muy alta.
    public static NivelIcr Clasificar(decimal total) => total switch
    {
        < 40 => NivelIcr.Baja,
        < 70 => NivelIcr.Media,
        < 90 => NivelIcr.Alta,
        _ => NivelIcr.MuyAlta,
    };

    // Lleva el puntaje base de un criterio a su peso en la base de datos.
    private static decimal Escalar(decimal puntosBase, string criterio, Func<string, decimal> pesoDe)
    {
        var maximo = PuntajeMaximo[criterio];
        return Math.Round(puntosBase * pesoDe(criterio) / maximo, 2);
    }
}

// Fuente desde la que se obtuvo la ubicación del reporte.
public enum FuenteUbicacion
{
    Gps,
    Manual,
    Otra,
}

// Estado de la denuncia formal ante una autoridad.
public enum EstadoDenuncia
{
    Si,
    EnProceso,
    No,
    SinInformar,
}

// Niveles de confiabilidad según el puntaje total.
public enum NivelIcr
{
    Baja,
    Media,
    Alta,
    MuyAlta,
}

// Campo del formulario evaluado para la completitud.
// EsRelevante marca los campos que cuentan para el nivel de completitud más alto.
public sealed record CampoCompletitud(string Nombre, bool EsRelevante, bool TieneDato);

// Datos que necesita el cálculo del ICR, ya leídos del reporte.
public sealed record EntradaIcr(
    IReadOnlyList<CampoCompletitud> Campos,
    bool FechaYUbicacionValidas,
    FuenteUbicacion Fuente,
    TimeSpan? TiempoTranscurrido,
    bool TiempoEsHoraExacta,
    EstadoDenuncia Denuncia,
    bool TieneRadicado,
    bool ConsistenciaOk);

// Resultado del cálculo: puntaje total, nivel y puntaje de cada criterio.
public sealed record ResultadoIcr(decimal Total, NivelIcr Nivel, IReadOnlyDictionary<string, decimal> Puntajes);
