FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:d5633778c754e89a386882eddf8dbee6942447a8a4b8267c19cf1e1158d0b0ac AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:0d99ebf1ccad15037b5c98c9fe8979d6b8dbac23812feae2277a0b277b5c5ffa AS build
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
