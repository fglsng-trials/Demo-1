# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apps/App.Consumer/App.Consumer.csproj", "Apps/App.Consumer/"]
COPY ["Abstractions/Abstractions.csproj", "Abstractions/"]
COPY ["Apps/Libraries/Lib.Consumer/Lib.Consumer.csproj", "Apps/Libraries/Lib.Consumer/"]
COPY ["Services/Service.DataStorage/Service.DataStorage.csproj", "Services/Service.DataStorage/"]
COPY ["Services/Service.IntermediateCache/Service.IntermediateCache.csproj", "Services/Service.IntermediateCache/"]
COPY ["Services/Service.MessageBroker/Service.MessageBroker.csproj", "Services/Service.MessageBroker/"]
COPY ["Apps/Libraries/Lib.Shared/Lib.Shared.csproj", "Apps/Libraries/Lib.Shared/"]
RUN dotnet restore "./Apps/App.Consumer/App.Consumer.csproj"
COPY . .
WORKDIR "/src/Apps/App.Consumer"
RUN dotnet build "./App.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Consumer.dll"]