using AppRobos.Backend.Catalogos;
using AppRobos.Backend.Modelos;
using AppRobos.Backend.Reportes;
using AppRobos.Backend.Territorio;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppRobos.Backend.Datos;

// Contexto de base de datos de la aplicación.
// Usa las tablas de ASP.NET Core Identity con nombres en español
// y agrega las tablas de estados y catálogos.
public class ContextoAplicacion : IdentityDbContext<CuentaUsuario, RolAplicacion, Guid>
{
    // Identificador fijo del rol Ciudadano. Fijo para que las migraciones siempre le den el mismo valor.
    public static readonly Guid IdRolCiudadano = Guid.Parse("6f1c2a4e-1b7d-4c2a-9a1e-5d3f0b8c7a01");

    // Identificador fijo del rol SuperAdmin. Puede leer los reportes de todos los ciudadanos.
    public static readonly Guid IdRolSuperAdmin = Guid.Parse("b3e8d1f0-4a6c-4e2b-8d7f-9c1a5e3b2d64");

    public ContextoAplicacion(DbContextOptions<ContextoAplicacion> opciones) : base(opciones)
    {
    }

    // Estados posibles de las cuentas.
    public DbSet<EstadoCuenta> EstadosCuenta => Set<EstadoCuenta>();

    // Opciones de las listas desplegables.
    public DbSet<CatalogoOpcion> CatalogosOpciones => Set<CatalogoOpcion>();

    // Políticas de rate limiting que se recargan en memoria cada 60 segundos.
    public DbSet<PoliticaLimiteTasa> PoliticasLimiteTasa => Set<PoliticaLimiteTasa>();

    // Reportes ciudadanos y sus secciones
    public DbSet<Reporte> Reportes => Set<Reporte>();

    // Criterios del ICR con su peso
    public DbSet<CriterioIcr> CriteriosIcr => Set<CriterioIcr>();

    // Territorio: municipios, comunas, barrios y área de cobertura
    public DbSet<TerritorioMunicipio> TerritorioMunicipios => Set<TerritorioMunicipio>();
    public DbSet<TerritorioComuna> TerritorioComunas => Set<TerritorioComuna>();
    public DbSet<TerritorioBarrio> TerritorioBarrios => Set<TerritorioBarrio>();
    public DbSet<CoberturaMetropolitana> CoberturaMetropolitana => Set<CoberturaMetropolitana>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cuenta de usuario: campos propios y relación con el estado
        modelBuilder.Entity<CuentaUsuario>(entidad =>
        {
            entidad.ToTable("CuentasUsuario");
            entidad.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            entidad.Property(c => c.Apellido).HasMaxLength(100).IsRequired();
            entidad.Property(c => c.SexoCodigo).HasMaxLength(40).IsRequired();
            entidad.Property(c => c.EstadoCodigo).HasMaxLength(20).IsRequired();
            entidad.Property(c => c.NivelBloqueo).HasDefaultValue(0);
            entidad.Property(c => c.CuentaDeshabilitada).HasDefaultValue(false);

            // Solo se permiten estados que existen en la tabla EstadosCuenta
            entidad.HasOne<EstadoCuenta>()
                   .WithMany()
                   .HasForeignKey(c => c.EstadoCodigo)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // Nombres en español para las tablas de Identity
        modelBuilder.Entity<RolAplicacion>().ToTable("RolesAplicacion");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("CuentasUsuarioRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("CuentasUsuarioClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("CuentasUsuarioLogins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("CuentasUsuarioTokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RolesAplicacionClaims");

        // Estados de cuenta
        modelBuilder.Entity<EstadoCuenta>(entidad =>
        {
            entidad.ToTable("EstadosCuenta");
            entidad.HasKey(e => e.Codigo);
            entidad.Property(e => e.Codigo).HasMaxLength(20);
            entidad.Property(e => e.Descripcion).HasMaxLength(120).IsRequired();
        });

        // Opciones de catálogo: el par (Catalogo, Codigo) no puede repetirse
        modelBuilder.Entity<CatalogoOpcion>(entidad =>
        {
            entidad.ToTable("CatalogoOpciones");
            entidad.Property(o => o.Catalogo).HasMaxLength(60).IsRequired();
            entidad.Property(o => o.Codigo).HasMaxLength(60).IsRequired();
            entidad.Property(o => o.Etiqueta).HasMaxLength(120).IsRequired();
            entidad.HasIndex(o => new { o.Catalogo, o.Codigo }).IsUnique();
        });

        // Políticas de rate limiting: límites válidos y nombre único
        modelBuilder.Entity<PoliticaLimiteTasa>(entidad =>
        {
            entidad.ToTable("PoliticasLimiteTasa", tabla =>
            {
                // Límites razonables: evitan configuraciones que desactiven la protección por accidente
                tabla.HasCheckConstraint("CK_PoliticasLimiteTasa_Limite", "\"LimitePeticiones\" BETWEEN 1 AND 10000");
                tabla.HasCheckConstraint("CK_PoliticasLimiteTasa_Ventana", "\"VentanaSegundos\" BETWEEN 1 AND 3600");
                tabla.HasCheckConstraint("CK_PoliticasLimiteTasa_Cola", "\"LimiteCola\" = 0");
            });
            entidad.Property(p => p.Nombre).HasMaxLength(60).IsRequired();
            entidad.Property(p => p.EstrategiaParticion).HasMaxLength(20).IsRequired();
            entidad.HasIndex(p => p.Nombre).IsUnique();
        });

        ConfigurarReportes(modelBuilder);
        ConfigurarTerritorio(modelBuilder);

        SembrarDatosIniciales(modelBuilder);
    }

    // Reglas de las tablas del reporte: claves, relaciones y tipos de columna.
    private static void ConfigurarReportes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reporte>(e =>
        {
            e.ToTable("Reporte", t => t.HasCheckConstraint("CK_Reporte_Estado", "\"EstadoReporte\" IN ('BORRADOR', 'ENVIADO')"));
            e.HasKey(r => r.Id);
            e.Property(r => r.EstadoReporte).HasMaxLength(12).IsRequired();
            e.Property(r => r.NumeroRadicado).HasMaxLength(50);
            e.Property(r => r.IcrPuntaje).HasPrecision(5, 2);
            e.Property(r => r.IcrClasificacion).HasMaxLength(20);
            e.HasIndex(r => new { r.UsuarioId, r.FechaActualizacion });
            e.HasOne<CuentaUsuario>().WithMany().HasForeignKey(r => r.UsuarioId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<TerritorioMunicipio>().WithMany().HasForeignKey(r => r.MunicipioCodigo).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<TerritorioBarrio>().WithMany().HasForeignKey(r => r.BarrioId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReporteBien>(e =>
        {
            e.ToTable("ReporteBien");
            e.HasKey(b => b.Id);
            e.HasOne(b => b.Reporte).WithMany(r => r.Bienes).HasForeignKey(b => b.ReporteId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(b => b.Celular).WithOne(c => c.Bien).HasForeignKey<ReporteBienCelular>(c => c.ReporteBienId);
            e.HasOne(b => b.Vehiculo).WithOne(v => v.Bien).HasForeignKey<ReporteBienVehiculo>(v => v.ReporteBienId);
            e.HasOne(b => b.Tarjeta).WithOne(t => t.Bien).HasForeignKey<ReporteBienTarjeta>(t => t.ReporteBienId);
            e.HasOne(b => b.Documento).WithOne(d => d.Bien).HasForeignKey<ReporteBienDocumento>(d => d.ReporteBienId);
        });

        modelBuilder.Entity<ReporteBienCelular>(e => { e.ToTable("ReporteBienCelular"); e.HasKey(c => c.ReporteBienId); });
        modelBuilder.Entity<ReporteBienVehiculo>(e => { e.ToTable("ReporteBienVehiculo"); e.HasKey(v => v.ReporteBienId); });
        modelBuilder.Entity<ReporteBienTarjeta>(e => { e.ToTable("ReporteBienTarjeta"); e.HasKey(t => t.ReporteBienId); });
        modelBuilder.Entity<ReporteBienDocumento>(e => { e.ToTable("ReporteBienDocumento"); e.HasKey(d => d.ReporteBienId); });

        modelBuilder.Entity<ReporteBienContenidoBolso>(e =>
        {
            e.ToTable("ReporteBienContenidoBolso");
            e.HasKey(c => new { c.ReporteBienId, c.Codigo });
            e.HasOne(c => c.Bien).WithMany(b => b.ContenidoBolso).HasForeignKey(c => c.ReporteBienId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReporteResponsables>(e => { e.ToTable("ReporteResponsables"); e.HasKey(x => x.ReporteId); });
        modelBuilder.Entity<ReporteDatosPersonales>(e => { e.ToTable("ReporteDatosPersonales"); e.HasKey(x => x.ReporteId); });

        modelBuilder.Entity<ReporteCaracteristicaSector>(e =>
        {
            e.ToTable("ReporteCaracteristicaSector");
            e.HasKey(x => new { x.ReporteId, x.Codigo });
        });

        modelBuilder.Entity<ReporteAccesoForzado>(e =>
        {
            e.ToTable("ReporteAccesoForzado");
            e.HasKey(x => new { x.ReporteId, x.Codigo });
        });

        modelBuilder.Entity<ReporteIcr>(e =>
        {
            e.ToTable("ReporteIcr");
            e.HasKey(x => new { x.ReporteId, x.Criterio });
            e.Property(x => x.Puntaje).HasPrecision(5, 2);
        });
    }

    // Reglas de las tablas de territorio y de criterios del ICR.
    private static void ConfigurarTerritorio(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritorioMunicipio>(e =>
        {
            e.ToTable("TerritorioMunicipios");
            e.HasKey(m => m.Codigo);
        });

        modelBuilder.Entity<TerritorioComuna>(e =>
        {
            e.ToTable("TerritorioComunas");
            e.HasKey(c => c.Id);
        });

        modelBuilder.Entity<TerritorioBarrio>(e =>
        {
            e.ToTable("TerritorioBarrios");
            e.HasKey(b => b.Id);
            e.HasIndex(b => b.Nombre);
            e.HasOne(b => b.Municipio).WithMany().HasForeignKey(b => b.MunicipioCodigo).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(b => b.Comuna).WithMany().HasForeignKey(b => b.ComunaId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CoberturaMetropolitana>(e =>
        {
            e.ToTable("CoberturaMetropolitana");
            e.HasKey(c => c.Id);
        });

        modelBuilder.Entity<CriterioIcr>(e =>
        {
            e.ToTable("CriteriosIcr", t => t.HasCheckConstraint("CK_CriteriosIcr_Peso", "\"Peso\" >= 0"));
            e.HasKey(c => c.Codigo);
            e.Property(c => c.Peso).HasPrecision(6, 2);
        });
    }


    // Carga los datos iniciales que la aplicación necesita para funcionar:
    // el rol Ciudadano, los estados de cuenta y la lista de sexo/género.
    // Se aplican con la migración, no al arrancar la API.
    private static void SembrarDatosIniciales(ModelBuilder modelBuilder)
    {
        // Roles de la aplicación: Ciudadano y SuperAdmin
        modelBuilder.Entity<RolAplicacion>().HasData(
            new RolAplicacion
            {
                Id = IdRolCiudadano,
                Name = RolesAplicacion.Ciudadano,
                NormalizedName = RolesAplicacion.Ciudadano.ToUpperInvariant(),
                ConcurrencyStamp = "rol-ciudadano-inicial"
            },
            new RolAplicacion
            {
                Id = IdRolSuperAdmin,
                Name = RolesAplicacion.SuperAdmin,
                NormalizedName = RolesAplicacion.SuperAdmin.ToUpperInvariant(),
                ConcurrencyStamp = "rol-superadmin-inicial"
            });

        // Estados posibles de una cuenta
        modelBuilder.Entity<EstadoCuenta>().HasData(
            new EstadoCuenta { Codigo = EstadosCuentaCodigos.Activa, Descripcion = "Cuenta activa; puede usar la aplicación.", Habilitado = true },
            new EstadoCuenta { Codigo = EstadosCuentaCodigos.Inactiva, Descripcion = "Cuenta inactiva; no puede iniciar sesión.", Habilitado = true },
            new EstadoCuenta { Codigo = EstadosCuentaCodigos.Bloqueada, Descripcion = "Cuenta bloqueada temporalmente por intentos fallidos.", Habilitado = true }
        );

        // Opciones de sexo/género. Son las únicas permitidas en el registro.
        modelBuilder.Entity<CatalogoOpcion>().HasData(
            new CatalogoOpcion { Id = 1, Catalogo = CatalogosAplicacion.SexoGenero, Codigo = "MASCULINO", Etiqueta = "Masculino", Orden = 1, Habilitado = true, CuentaComoDato = true },
            new CatalogoOpcion { Id = 2, Catalogo = CatalogosAplicacion.SexoGenero, Codigo = "FEMENINO", Etiqueta = "Femenino", Orden = 2, Habilitado = true, CuentaComoDato = true },
            new CatalogoOpcion { Id = 3, Catalogo = CatalogosAplicacion.SexoGenero, Codigo = "PREFIERO_NO_DECIR", Etiqueta = "Prefiero no decirlo", Orden = 3, Habilitado = true, CuentaComoDato = true }
        );

        // Políticas de rate limiting iniciales. Deben coincidir con los valores de respaldo del servicio.
        var fechaInicial = new DateTimeOffset(2026, 10, 30, 0, 0, 0, TimeSpan.Zero);
        modelBuilder.Entity<PoliticaLimiteTasa>().HasData(
            new PoliticaLimiteTasa { Id = 1, Nombre = "PoliticaLogin", LimitePeticiones = 5, VentanaSegundos = 60, EstrategiaParticion = "IP", LimiteCola = 0, Activa = true, FechaCreacion = fechaInicial, FechaActualizacion = fechaInicial },
            new PoliticaLimiteTasa { Id = 2, Nombre = "PoliticaRegistro", LimitePeticiones = 3, VentanaSegundos = 60, EstrategiaParticion = "IP", LimiteCola = 0, Activa = true, FechaCreacion = fechaInicial, FechaActualizacion = fechaInicial },
            new PoliticaLimiteTasa { Id = 3, Nombre = "PoliticaLecturaReportes", LimitePeticiones = 60, VentanaSegundos = 60, EstrategiaParticion = "IP", LimiteCola = 0, Activa = true, FechaCreacion = fechaInicial, FechaActualizacion = fechaInicial },
            new PoliticaLimiteTasa { Id = 4, Nombre = "PoliticaPorDefecto", LimitePeticiones = 100, VentanaSegundos = 60, EstrategiaParticion = "IP", LimiteCola = 0, Activa = true, FechaCreacion = fechaInicial, FechaActualizacion = fechaInicial }
        );

        // Listas del formulario. Los ids empiezan en 100 y se asignan en orden alfabético de catálogo
        // para que las migraciones sean estables.
        var siguienteId = 100;
        var opcionesFormulario = new List<CatalogoOpcion>();
        foreach (var (catalogo, opciones) in DefinicionesCatalogos.Todos.OrderBy(k => k.Key, StringComparer.Ordinal))
        {
            for (var indice = 0; indice < opciones.Count; indice++)
            {
                opcionesFormulario.Add(new CatalogoOpcion
                {
                    Id = siguienteId++,
                    Catalogo = catalogo,
                    Codigo = opciones[indice].Codigo,
                    Etiqueta = opciones[indice].Etiqueta,
                    Orden = indice + 1,
                    Habilitado = true,
                    CuentaComoDato = opciones[indice].CuentaComoDato,
                });
            }
        }
        modelBuilder.Entity<CatalogoOpcion>().HasData(opcionesFormulario);

        // Municipios del Valle de Aburrá que puede elegir el ciudadano
        modelBuilder.Entity<TerritorioMunicipio>().HasData(
            new TerritorioMunicipio { Codigo = "MEDELLIN", Nombre = "Medellín" },
            new TerritorioMunicipio { Codigo = "BELLO", Nombre = "Bello" },
            new TerritorioMunicipio { Codigo = "ITAGUI", Nombre = "Itagüí" },
            new TerritorioMunicipio { Codigo = "ENVIGADO", Nombre = "Envigado" },
            new TerritorioMunicipio { Codigo = "SABANETA", Nombre = "Sabaneta" },
            new TerritorioMunicipio { Codigo = "LA_ESTRELLA", Nombre = "La Estrella" },
            new TerritorioMunicipio { Codigo = "CALDAS", Nombre = "Caldas" },
            new TerritorioMunicipio { Codigo = "COPACABANA", Nombre = "Copacabana" },
            new TerritorioMunicipio { Codigo = "GIRARDOTA", Nombre = "Girardota" },
            new TerritorioMunicipio { Codigo = "BARBOSA", Nombre = "Barbosa" }
        );

        // Criterios del ICR. La suma de los activos debe ser 100 (lo valida la base de datos).
        modelBuilder.Entity<CriterioIcr>().HasData(
            new CriterioIcr { Codigo = "COMPLETITUD", Nombre = "Completitud", Peso = 25, Activo = true },
            new CriterioIcr { Codigo = "UBICACION", Nombre = "Precisión de la ubicación", Peso = 20, Activo = true },
            new CriterioIcr { Codigo = "TIEMPO", Nombre = "Tiempo transcurrido", Peso = 30, Activo = true },
            new CriterioIcr { Codigo = "DENUNCIA", Nombre = "Denuncia", Peso = 10, Activo = true },
            new CriterioIcr { Codigo = "RADICADO", Nombre = "Radicado", Peso = 10, Activo = true },
            new CriterioIcr { Codigo = "CONSISTENCIA", Nombre = "Consistencia y validez", Peso = 5, Activo = true }
        );
    }
}


