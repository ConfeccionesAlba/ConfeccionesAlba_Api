FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:eaa79205c3ade4792a7f7bf310a3aac51fe0e1d91c44e40f70b7c6423d475fe0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:d1823fecac3689a2eb959e02ee3bfe1c2142392808240039097ad70644566190 AS build
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
