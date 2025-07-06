# ----------------------------
# STAGE 1: Build
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all content from the current build context (where Dockerfile is located)
# to the /src directory inside the container.
COPY . .

# Change working directory to /src where your .csproj file now resides.
WORKDIR /src 

# Restore dependencies for the project
RUN dotnet restore PlantBiologyEducation.csproj 

# Build and publish the project
RUN dotnet publish PlantBiologyEducation.csproj -c Release -o /app/publish 

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