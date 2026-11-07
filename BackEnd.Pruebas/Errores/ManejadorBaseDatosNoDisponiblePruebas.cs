using System.Net.Sockets;
using AppRobos.Backend.Errores;
using Npgsql;
using Xunit;

namespace BackEnd.Pruebas.Errores;

// Pruebas de la detección de fallos de conexión a la base de datos.
public class ManejadorBaseDatosNoDisponiblePruebas
{
    [Fact]
    public void ErrorDeNpgsqlEsFalloDeConexion()
    {
        var excepcion = new NpgsqlException("No se pudo conectar");

        Assert.True(ManejadorBaseDatosNoDisponible.EsFalloDeConexion(excepcion));
    }

    [Fact]
    public void ErrorDeSocketAnidadoEsFalloDeConexion()
    {
        // Así llega normalmente: un SocketException dentro de un NpgsqlException
        var excepcion = new NpgsqlException("No se pudo conectar", new SocketException());

        Assert.True(ManejadorBaseDatosNoDisponible.EsFalloDeConexion(excepcion));
    }

    [Fact]
    public void ErrorDeNegocioNoEsFalloDeConexion()
    {
        Assert.False(ManejadorBaseDatosNoDisponible.EsFalloDeConexion(new InvalidOperationException("regla de negocio")));
    }

    [Fact]
    public void FalloDeConexionDentroDeOtroErrorTambienSeDetecta()
    {
        var excepcion = new InvalidOperationException("envoltorio", new NpgsqlException("sin conexión"));

        Assert.True(ManejadorBaseDatosNoDisponible.EsFalloDeConexion(excepcion));
    }
}
