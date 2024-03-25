# This Dockerfile contains Build and Release steps:
# 1. Build image(https://hub.docker.com/_/microsoft-dotnet-core-sdk/)
FROM mcr.microsoft.com/dotnet/sdk:8.0.101-alpine3.18-amd64 AS build
WORKDIR /source

# Cache nuget restore
COPY /src/Echo/*.csproj .
RUN dotnet restore Echo.csproj

# Copy sources and compile
COPY /src/Echo .
RUN dotnet publish Echo.csproj --output /app/ --configuration Release

# 2. Release image
FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-alpine3.18-amd64
WORKDIR /app

ENV ASPNETCORE_URLS http://*:8080
EXPOSE 8080

# Copy content from Build image
COPY --from=build /app .

ENTRYPOINT ["dotnet", "Echo.dll"]
