using Npgsql;
using System.Text;
using AppRobos.Backend.Configuracion;
using AppRobos.Backend.Catalogos;
using AppRobos.Backend.Datos;
using AppRobos.Backend.Errores;
using AppRobos.Backend.Modelos;
using AppRobos.Backend.Reportes;
using AppRobos.Backend.Seguridad;
using AppRobos.Backend.Servicios;
using AppRobos.Backend.Territorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Opciones validadas al arrancar: si falta una clave o un valor inválido, la API no inicia
builder.Services.AddOptions<ConfiguracionJwt>()
    .Bind(builder.Configuration.GetSection(ConfiguracionJwt.NombreSeccion))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<ReglasBloqueo>()
    .Bind(builder.Configuration.GetSection(ReglasBloqueo.NombreSeccion))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Base de datos PostgreSQL con soporte geoespacial: el origen de datos de Npgsql
// usa NetTopologySuite para leer y escribir geometrías de PostGIS
var cadenaBaseDatos = builder.Configuration.GetConnectionString("BaseDatos")
    ?? throw new InvalidOperationException("Falta la cadena de conexión ConnectionStrings:BaseDatos.");
var fuenteDatos = new NpgsqlDataSourceBuilder(cadenaBaseDatos).Build();
builder.Services.AddSingleton(fuenteDatos);
// La sesión de cada petición se comunica a PostgreSQL al abrir cada conexión (para RLS)
builder.Services.AddScoped<ContextoSesionBaseDatos>();
builder.Services.AddScoped<InterceptorSesionBaseDatos>();
builder.Services.AddDbContext<ContextoAplicacion>((sp, opcionesBd) =>
    opcionesBd.UseNpgsql(fuenteDatos).AddInterceptors(sp.GetRequiredService<InterceptorSesionBaseDatos>()));

// ASP.NET Core Identity: contraseñas con hash, correo único y bloqueo de cuentas
builder.Services.AddIdentity<CuentaUsuario, RolAplicacion>(opciones =>
{
    // Política de contraseña: mínimo 8 caracteres, con mayúscula, minúscula y número
    opciones.Password.RequiredLength = 8;
    opciones.Password.RequireDigit = true;
    opciones.Password.RequireUppercase = true;
    opciones.Password.RequireLowercase = true;
    opciones.Password.RequireNonAlphanumeric = false;
    opciones.Password.RequiredUniqueChars = 1;

    // Un solo correo por cuenta
    opciones.User.RequireUniqueEmail = true;

    // El bloqueo escalonado lo controla ServicioBloqueoCuenta; Identity solo guarda LockoutEnd
    opciones.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ContextoAplicacion>()
.AddDefaultTokenProviders();

// Servicios de la aplicación
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ServicioBloqueoCuenta>();
builder.Services.AddSingleton<ServicioTokenJwt>();
builder.Services.AddScoped<ServicioCatalogos>();
builder.Services.AddScoped<ServicioAutenticacion>();
builder.Services.AddScoped<ServicioPerfil>();

// Reportes ciudadanos: listas del formulario, territorio, validación y casos de uso
builder.Services.AddScoped<ServicioOpcionesFormulario>();
builder.Services.AddScoped<ServicioTerritorio>();
builder.Services.AddScoped<ValidadorGuardadoReporte>();
builder.Services.AddScoped<ServicioReportes>();
builder.Services.AddDataProtection();
builder.Services.AddHostedService<BorradoresVencidosService>();

// Autenticación JWT: se validan firma, emisor, audiencia y vencimiento
var configuracionJwt = builder.Configuration
    .GetSection(ConfiguracionJwt.NombreSeccion)
    .Get<ConfiguracionJwt>() ?? new ConfiguracionJwt();

// Identity registra cookies por defecto; la API usa solo JWT, así que se fija JWT como esquema
// de autenticación y de desafío para que un token inválido responda 401 y no redirija al login
builder.Services.AddAuthentication(opciones =>
    {
        opciones.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        opciones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opciones.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opciones =>
    {
        // Mantener los nombres de claims tal como se emiten ("sub", "role", "email")
        opciones.MapInboundClaims = false;
        opciones.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

        // Cada token se revisa contra la cuenta: si cambió el sello, la sesión ya no es válida
        opciones.Events = new JwtBearerEvents { OnTokenValidated = AppRobos.Backend.Seguridad.ValidacionSesion.ValidarAsync };

        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuracionJwt.Emisor,
            ValidateAudience = true,
            ValidAudience = configuracionJwt.Audiencia,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracionJwt.Clave)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RoleClaimType = "role",
            NameClaimType = "email"
        };
    });

// Rate limiting: políticas guardadas en la BD, copiadas en memoria y recargadas cada 60 segundos.
// El limitador usa el middleware integrado de ASP.NET Core, no un contador propio.
builder.Services.AddSingleton<ServicioConfiguracionLimiteTasa>();
builder.Services.AddHostedService<TrabajoRecargaLimiteTasa>();
builder.Services.ConfigureOptions<ConfigurarLimitadorTasa>();
builder.Services.AddRateLimiter(_ => { });

builder.Services.AddAuthorization();

// Errores no controlados: si la base no responde se devuelve 503; el resto sigue el manejo normal
builder.Services.AddExceptionHandler<ManejadorBaseDatosNoDisponible>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con el botón "Authorize" para probar los endpoints protegidos con el token
builder.Services.AddSwaggerGen(opciones =>
{
    // Descripciones de endpoints tomadas de los comentarios /// del código
    opciones.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AppRobos.Backend.xml"));

    opciones.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v1.0",
        Title = "AppRobos Backend",
        Description = "API del MVP de consulta y análisis de hurtos en Medellín y el Valle de Aburrá."
    });

    opciones.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Description = "Autenticación JWT mediante Bearer. Escribe el token obtenido en el login.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    opciones.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", documento)] = new List<string>()
    });
});

var app = builder.Build();

// Primer middleware: captura los errores del resto del pipeline
app.UseExceptionHandler();

// La documentación interactiva solo está disponible en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// El límite va antes de la autenticación: así un ataque de fuerza bruta se corta sin verificar contraseñas
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Clase de arranque expuesta para las pruebas de integración.
public partial class Program
{
}



