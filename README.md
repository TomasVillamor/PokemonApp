# PokemonApp
REQUISITOS PREVIOS
.NET SDK 8.0+
.SQL Server
.Visual Studio 2022 o VS Code
.Git

INSTRUCCIONES PARA CORRER LA APP
1.Clonar el repositorio
2.En el archivo appsettings.json de PokemonApp.API, debes ingresar el nombre del servidor de tu instancia local de SQL Server en el campo "DefaultConnection".
Por ejemplo:
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PokemonDb;Trusted_Connection=True;TrustServerCertificate=True"
}
3.Luego ejecutar el comando: dotnet ef database update para crear la estructura de la base de datos
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
