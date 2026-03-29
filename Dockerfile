FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:a04d1c1d2d26119049494057d80ea6cda25bbd8aef7c444a1fc1ef874fd3955b AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:478b9038d187e5b5c29bfa8173ded5d29e864b5ad06102a12106380ee01e2e49 AS build
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
