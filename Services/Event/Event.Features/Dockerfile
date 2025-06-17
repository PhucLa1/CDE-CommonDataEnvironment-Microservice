#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Event/Event.Features/Event.Features.csproj", "Services/Event/Event.Features/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
COPY ["Services/Event/Event.Infrastructure/Event.Infrastructure.csproj", "Services/Event/Event.Infrastructure/"]
COPY ["Services/Event/Event.Core/Event.Core.csproj", "Services/Event/Event.Core/"]
COPY ["Services/Event/Event.Shared/Event.Shared.csproj", "Services/Event/Event.Shared/"]
RUN dotnet restore "./Services/Event/Event.Features/Event.Features.csproj"
COPY . .
WORKDIR "/src/Services/Event/Event.Features"
RUN dotnet build "./Event.Features.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Event.Features.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Event.Features.dll"]