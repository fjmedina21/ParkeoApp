# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Imagen para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ParkeoApp.Api/ParkeoApp.Api.csproj", "ParkeoApp.Api/"]
RUN dotnet restore "ParkeoApp.Api/ParkeoApp.Api.csproj"
COPY . .
WORKDIR "/src/ParkeoApp.Api"
RUN dotnet build "./ParkeoApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ParkeoApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Railway define el puerto mediante la variable de entorno $PORT
ENV ASPNETCORE_URLS=http://+:${PORT}
ENTRYPOINT ["dotnet", "ParkeoApp.Api.dll"]
