# AppRobos - Backend

API de AppRobos, una aplicación ciudadana para consultar y reportar hurtos en Medellín y el Valle de Aburrá.

## Tecnologías

- ASP.NET Core 10 (Web API)
- PostgreSQL con PostGIS
- Entity Framework Core 10 con Npgsql
- ASP.NET Core Identity con JWT
- xUnit para pruebas

## Estructura

- `BackEnd/`: proyecto de la API.
- `BackEnd.Pruebas/`: pruebas unitarias del ICR, las reglas del reporte, la autenticación y el rate limiting.
- `BackEnd/Migraciones/`: migraciones de EF Core. Incluyen SQL para PostGIS, RLS, disparadores y funciones.
- `Cartografia/`: GeoJSON de los límites del área metropolitana y de los barrios.

## Requisitos

- .NET SDK 10
- PostgreSQL con la extensión PostGIS (probado con PostgreSQL 18 y PostGIS 3.6)
- dotnet-ef 10.0.0:

```
dotnet tool install --global dotnet-ef --version 10.0.0
```

## Configuración

Los secretos no van en el repositorio. Se guardan con user-secrets desde la carpeta `BackEnd`:

```
dotnet user-secrets set "ConfiguracionJwt:Clave" "<mínimo 32 caracteres>"
dotnet user-secrets set "ConnectionStrings:BaseDatos" "Host=localhost;Port=5432;Database=AppRobos;Username=<rol de la API>;Password=<clave>"
dotnet user-secrets set "ConnectionStrings:BaseDatosMigraciones" "Host=localhost;Port=5432;Database=AppRobos;Username=<dueño de las tablas>;Password=<clave>"
```

- `ConnectionStrings:BaseDatos` es la cadena que usa la API. El rol debe ser distinto del superusuario: RLS no se aplica a los superusuarios.
- `ConnectionStrings:BaseDatosMigraciones` es el dueño de las tablas. Se usa solo para migraciones y tareas de administración.

`appsettings.json` solo tiene la cadena de conexión sin contraseña y las demás opciones de configuración.

## Base de datos

1. Crear la base `AppRobos` y habilitar PostGIS.
2. Crear el rol de la API y darle permisos sobre las tablas de la aplicación. Debe ser un rol sin privilegios de superusuario.
3. Aplicar las migraciones con el dueño de las tablas:

```
cd BackEnd
dotnet ef database update --connection "<valor de ConnectionStrings:BaseDatosMigraciones>"
```

Las migraciones crean las tablas, los roles `Ciudadano` y `SuperAdmin`, los estados de cuenta, los catálogos del formulario, los municipios, los criterios del ICR y las políticas de rate limiting.

Los polígonos de barrios, comunas y del área metropolitana no vienen en las migraciones. Se cargan aparte desde los GeoJSON de `Cartografia/`.

## Ejecución

```
dotnet build BackEnd/BackEnd.csproj
dotnet run --project BackEnd
```

En desarrollo, Swagger está disponible en `/swagger`.

## Pruebas

```
dotnet test BackEnd.Pruebas/BackEnd.Pruebas.csproj
```

## Endpoints

Todos requieren el encabezado `Authorization: Bearer <token>`, salvo el registro y el login.

| Ruta | Descripción |
|---|---|
| `POST /api/autenticacion/registro` | Registro de ciudadano |
| `POST /api/autenticacion/login` | Inicio de sesión |
| `GET /api/autenticacion/correo-disponible?correo=` | Indica si el correo está libre. Solo devuelve `disponible` |
| `GET /api/autenticacion/telefono-disponible?telefono=` | Indica si el celular está libre. Valida el formato con el DTO |
| `GET /api/autenticacion/perfil` | Perfil del usuario en sesión |
| `PUT /api/autenticacion/perfil` | Actualizar datos del perfil |
| `PUT /api/autenticacion/perfil/contrasena` | Cambiar contraseña (cierra todas las sesiones) |
| `POST /api/reportes` | Crear borrador |
| `GET /api/reportes` | Listar los reportes visibles para el usuario |
| `GET /api/reportes/{id}` | Ver un reporte |
| `PUT /api/reportes/{id}` | Editar un borrador |
| `DELETE /api/reportes/{id}` | Borrar un borrador |
| `POST /api/reportes/{id}/enviar` | Enviar el reporte y calcular el ICR |
| `GET /api/catalogos/municipios` | Municipios |
| `GET /api/catalogos/municipios/{codigo}/barrios?busqueda=` | Barrios de un municipio |
| `GET /api/catalogos/{catalogo}` | Opciones de una lista del formulario |
| `GET /api/territorio/ubicacion?latitud=&longitud=` | Municipio y barrio de un punto del mapa |

## Seguridad

- Roles: `Ciudadano` se asigna al registrarse. `SuperAdmin` se asigna en la base de datos y solo puede leer los reportes de todos los usuarios.
- Un ciudadano solo ve sus propios reportes. La restricción la aplica RLS en PostgreSQL con el id del token.
- El token se valida en cada petición contra la base. Cambiar la contraseña o deshabilitar la cuenta invalida las sesiones abiertas.
- Bloqueo escalonado tras intentos fallidos de inicio de sesión o de cambio de contraseña.
- El rate limiting usa políticas guardadas en la tabla `PoliticasLimiteTasa`, recargadas cada 60 segundos.
- Un reporte enviado no se puede modificar ni borrar. Un disparador en la base lo impide también desde fuera de la API.
- Los borradores vencen a los 15 días desde el primer guardado. Un proceso en segundo plano los borra cada hora.
- La cédula se cifra con ASP.NET Core Data Protection y nunca se devuelve. Las llaves de Data Protection deben persistirse en el despliegue.

## Pendientes

- Calcular municipio y barrio desde las coordenadas al guardar el reporte. Hoy la app los envía y la API los valida.
- Cargar los barrios de Girardota. Mientras no estén, el endpoint de ubicación devuelve el punto sin barrio.
- Definir el puntaje del tiempo para la franja horaria aproximada y el momento desconocido en el ICR.
- Definir cómo se eliminan las cuentas con reportes enviados.
- Definir el despliegue.
