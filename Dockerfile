# Etapa base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar toda la solución
COPY . .

# Ir al proyecto principal de la API
WORKDIR /src/PokemonApp.API

# Publicar el proyecto
RUN dotnet publish -c Release -o /app/publish

# Etapa final para ejecutar
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PokemonApp.API.dll"]
