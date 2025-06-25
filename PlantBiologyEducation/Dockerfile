# ----------------------------
# STAGE 1: Build
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY PlantBiologyEducation.csproj .
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish
RUN dotnet publish -c Release -o /app/publish

# ----------------------------
# STAGE 2: Runtime
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose default port
EXPOSE 80

# Entry point for the application
ENTRYPOINT ["dotnet", "PlantBiologyEducation.dll"]
