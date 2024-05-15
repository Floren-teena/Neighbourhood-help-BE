#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NeighbourhoodHelp.Api/NeighbourhoodHelp.Api.csproj", "NeighbourhoodHelp.Api/"]
COPY ["NeighbourhoodHelp.Core/NeighbourhoodHelp.Core.csproj", "NeighbourhoodHelp.Core/"]
COPY ["NeighbourhoodHelp.Data/NeighbourhoodHelp.Data.csproj", "NeighbourhoodHelp.Data/"]
COPY ["NeighbourhoodHelp.Infrastructure/NeighbourhoodHelp.Infrastructure.csproj", "NeighbourhoodHelp.Infrastructure/"]
COPY ["NeighbourhoodHelp.Model/NeighbourhoodHelp.Model.csproj", "NeighbourhoodHelp.Model/"]
RUN dotnet restore "./NeighbourhoodHelp.Api/NeighbourhoodHelp.Api.csproj"
COPY . .
WORKDIR "/src/NeighbourhoodHelp.Api"
RUN dotnet build "./NeighbourhoodHelp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NeighbourhoodHelp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NeighbourhoodHelp.Api.dll"]