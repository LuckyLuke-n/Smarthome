# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src-docker/Smarthome.Api/Smarthome.Api.csproj", "src-docker/Smarthome.Api/"]
COPY ["src-dotnet/Smarthome.Core/Smarthome.Core.csproj", "src-dotnet/Smarthome.Core/"]
COPY ["src-dotnet/LSoftware.Communication.Abstractions/LSoftware.Communication.Abstractions.csproj", "src-dotnet/LSoftware.Communication.Abstractions/"]
COPY ["src-dotnet/LSoftware.Communication.Extensions/LSoftware.Communication.Extensions.csproj", "src-dotnet/LSoftware.Communication.Extensions/"]
COPY ["src-dotnet/LSoftware.Communication.Mqtt/LSoftware.Communication.Mqtt.csproj", "src-dotnet/LSoftware.Communication.Mqtt/"]
COPY ["src-dotnet/LSoftware.Metrics.Abstractions/LSoftware.Metrics.Abstractions.csproj", "src-dotnet/LSoftware.Metrics.Abstractions/"]
COPY ["src-dotnet/LSoftware.Metrics.Extensions/LSoftware.Metrics.Extensions.csproj", "src-dotnet/LSoftware.Metrics.Extensions/"]
COPY ["src-dotnet/LSoftware.Metrics.Influx/LSoftware.Metrics.Influx.csproj", "src-dotnet/LSoftware.Metrics.Influx/"]

RUN dotnet restore "./src-docker/Smarthome.Api/Smarthome.Api.csproj"
COPY . .
WORKDIR "/src/src-docker/Smarthome.Api"
RUN dotnet build "./Smarthome.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Smarthome.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smarthome.Api.dll"]