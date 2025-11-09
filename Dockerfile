FROM mcr.microsoft.com/dotnet/aspnet:9.0@sha256:3dcb33395722d14c80d19107158293ed677b2c07841100d51df07275ae2b2682 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:81f6d622fe21ed9d31375167f62a3538ff4d6835f9d5e6da9c2defa8a84b7687 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ConfeccionesAlba_Api/ConfeccionesAlba_Api.csproj", "src/ConfeccionesAlba_Api/"]
RUN dotnet restore "src/ConfeccionesAlba_Api/ConfeccionesAlba_Api.csproj"
COPY . .
WORKDIR "/src/src/ConfeccionesAlba_Api"
RUN dotnet build "./ConfeccionesAlba_Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConfeccionesAlba_Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfeccionesAlba_Api.dll"]
