FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Photon.ImageDb.csproj", "Photon.ImageDb/"]
RUN dotnet restore "Photon.ImageDb/Photon.ImageDb.csproj"
COPY . Photon.ImageDb
WORKDIR "/src/Photon.ImageDb"
RUN dotnet build "Photon.ImageDb.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Photon.ImageDb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Photon.ImageDb.dll"]
