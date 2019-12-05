# This Dockerfile contains Build and Release steps:
# 1. Build image(https://hub.docker.com/_/microsoft-dotnet-core-sdk/)
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-alpine3.10 AS build
WORKDIR /source

# Cache nuget restore
COPY /src/Echo/*.csproj .
RUN dotnet restore Echo.csproj

# Copy sources and compile
COPY /src/Echo .
RUN dotnet publish Echo.csproj --output /app/ --configuration Release

# 2. Release image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.0-alpine3.10
WORKDIR /app
EXPOSE 80

# Copy content from Build image
COPY --from=build /app .

ENTRYPOINT ["dotnet", "Echo.dll"]
