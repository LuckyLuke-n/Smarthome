FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src-docker/Smarthome.AmbientCollector.Api/Smarthome.AmbientCollector.Api.csproj", "src-docker/Smarthome.AmbientCollector.Api/"]
COPY ["src-dotnet/Smarthome.ServiceDefaults/Smarthome.ServiceDefaults.csproj", "src-dotnet/Smarthome.ServiceDefaults/"]
COPY ["src-dotnet/LSoftware.Communication.Mqtt/LSoftware.Communication.Mqtt.csproj", "src-dotnet/LSoftware.Communication.Mqtt/"]
COPY ["src-dotnet/LSoftware.Communication.Abstractions/LSoftware.Communication.Abstractions.csproj", "src-dotnet/LSoftware.Communication.Abstractions/"]
COPY ["src-dotnet/LSoftware.Repository.MongoDb/LSoftware.Repository.MongoDb.csproj", "src-dotnet/LSoftware.Repository.MongoDb/"]
COPY ["src-dotnet/LSoftware.Repository.Abstractions/LSoftware.Repository.Abstractions.csproj", "src-dotnet/LSoftware.Repository.Abstractions/"]
COPY ["src-dotnet/Smarthome.Core/Smarthome.Core.csproj", "src-dotnet/Smarthome.Core/"]
RUN dotnet restore "src-docker/Smarthome.AmbientCollector.Api/Smarthome.AmbientCollector.Api.csproj"
COPY . .
WORKDIR "/src/src-docker/Smarthome.AmbientCollector.Api"
RUN dotnet build "./Smarthome.AmbientCollector.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Smarthome.AmbientCollector.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AmbientCollector.dll"]
