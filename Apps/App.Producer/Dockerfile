# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apps/App.Producer/App.Producer.csproj", "Apps/App.Producer/"]
COPY ["Abstractions/Abstractions.csproj", "Abstractions/"]
COPY ["Services/Service.IntermediateCache/Service.IntermediateCache.csproj", "Services/Service.IntermediateCache/"]
COPY ["Services/Service.MessageBroker/Service.MessageBroker.csproj", "Services/Service.MessageBroker/"]
COPY ["Apps/Libraries/Lib.Shared/Lib.Shared.csproj", "Apps/Libraries/Lib.Shared/"]
RUN dotnet restore "./Apps/App.Producer/App.Producer.csproj"
COPY . .
WORKDIR "/src/Apps/App.Producer"
RUN dotnet build "./App.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Producer.dll"]