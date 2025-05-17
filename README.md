# PokemonApp
REQUISITOS PREVIOS
.NET SDK 8.0+
.SQL Server
.Visual Studio 2022 o VS Code
.Git

INSTRUCCIONES PARA CORRER LA APP
1.Clonar el repositorio
2.Debes crear el archivo appsettings.Development.json en PokemonApp.API y realizar lo siguiente:
Copiar y pegar este texto:
 {
 "ConnectionStrings": {
    "DefaultConnection": "Server=;Database=PokemonAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AdminUser": {
    "Email": "admin@test.com",
    "Username": "admin",
    "Password": "admin123"
  },
  "Jwt": {
    "Key": "b3d#0GzP!rF6aQ@vLw8zYc2jN0kE3hU5XvM9qSbT1gR7dLuCcHsPaKzWxEfVbGyT",
    "Issuer": "PokemonAPI",
    "Audience": "PokemonUsers",
    "ExpiresInMinutes": 60
  }
  }
En "ConnectionStrings" en el campo "Server" ingresar el nombre del servidor de tu instacia local de SQL server
3.Luego ejecutar el comando: dotnet ef database update para crear la estructura de la base de datos o desde el vs abrir el administrador de paquetes, elegir el proyecto PokemonApp.DataAcess y ejecutar el comando update database
4.Por ultimo, ejecutar la aplicacion desde el VS o dotnet run --project PokemonApp.API. Accede a la documentacion en el swagger https://localhost:{puerto}/swagger

DOCUMENTACION DE LA APP

Descripción General
La API permite gestionar una base de datos local de Pokémon, trayendo informacion externa de la PokeAPI. Además, se incluye autenticación JWT con roles (Admin, User) para proteger ciertos endpoints.

Datos Iniciales y Usuario por Defecto
Se crea automáticamente un usuario administrador por defecto en la base de datos, con las siguientes credenciales: 
Username = "admin", Email = "admin@test.com" y password = "admin123"

Restricciones
No se permiten modificaciones en pokemones traidos desde la API externa, en cambio, si se crean pokemones "manualmente" si se pueden modificar. 
Esta decisión se debe para preservar la integridad de los datos sincronizados, ya que son de una fuente externa.

Autenticación
Se genera un JWT y se debe usar en el header Authorization: Bearer <token>.
Los endpoints estan protegidos como se menciona en el challenge.

Endpoins

AuthController
POST /api/auth/register
Registra un nuevo usuario con email, username y password

POST /api/auth/login
Inicia sesión. Devuelve un JWT si las credenciales son válidas.

PokemonController
GET /api/pokemon
Lista todos los Pokémon registrados en la base de datos.

GET /api/pokemon/{id}
Obtiene los detalles de un Pokémon por su ID.

POST /api/pokemon
Crea un nuevo Pokémon en la base de datos.

PUT /api/pokemon/{id}
Actualiza un Pokémon existente.

DELETE /api/pokemon/{id}
Elimina un Pokémon por ID.

POST /api/pokemon/sync
Sincroniza datos desde la PokéAPI. Trae todos los pokemones de la API externa, si eliminas alguno y sincronizas depues te trae el eliminado.


Ingeniera de sotware
Se realiazo mediante la arquitecura de capas. Cada una conoce hacia "abajo", no esta permitido conocer a la de "arriba"
PokemonApp.API -> Capa de presentación (controladores, configuración)
PokemonApp.Services -> Lógica de negocio (servicios)
PokemonApp.DataAccess -> Acceso a datos (repositorios, EF)
PokemonApp.Domain -> Modelos de dominio
PokemonApp.Tests -> Tests automatizados

Tecnologias
.NET 8	Framework principal
Entity Framework Core	ORM para acceso a datos
JWT	Autenticación
xUnit y Moq	Tests
Swagger	Documentación de la API
