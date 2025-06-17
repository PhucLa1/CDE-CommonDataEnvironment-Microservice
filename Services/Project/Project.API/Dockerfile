#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Project/Project.API/Project.API.csproj", "Services/Project/Project.API/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
COPY ["Services/Project/Project.Application/Project.Application.csproj", "Services/Project/Project.Application/"]
COPY ["Services/Project/Project.Domain/Project.Domain.csproj", "Services/Project/Project.Domain/"]
COPY ["Services/Project/Project.Infrastructure/Project.Infrastructure.csproj", "Services/Project/Project.Infrastructure/"]
RUN dotnet restore "./Services/Project/Project.API/Project.API.csproj"
COPY . .
WORKDIR "/src/Services/Project/Project.API"
RUN dotnet build "./Project.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Project.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.API.dll"]