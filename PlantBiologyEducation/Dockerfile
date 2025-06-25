# ----------------------------
# STAGE 1: Build
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire project folder from the root of the context
# This assumes your .csproj and source files are inside a subfolder named PlantBiologyEducation
# in your GitHub repo.
COPY PlantBiologyEducation/ ./PlantBiologyEducation/

# Change working directory inside the container to the project folder
WORKDIR /src/PlantBiologyEducation

# Restore dependencies for the project in the current working directory
RUN dotnet restore

# Build and publish the project in the current working directory
RUN dotnet publish -c Release -o /app/publish

# ----------------------------
# STAGE 2: Runtime
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose port for Render deployment
EXPOSE 8080

# Set environment variable for ASP.NET Core to listen on all interfaces
ENV ASPNETCORE_URLS=http://+:8080

# Entry point for the application
ENTRYPOINT ["dotnet", "PlantBiologyEducation.dll"]