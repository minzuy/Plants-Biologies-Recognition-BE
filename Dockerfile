# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj và restore
COPY ["PlantBiologyEducation/PlantBiologyEducation.csproj", "PlantBiologyEducation/"]
RUN dotnet restore "PlantBiologyEducation/PlantBiologyEducation.csproj"

# Copy toàn bộ source code
COPY PlantBiologyEducation/. ./PlantBiologyEducation/

# Build và publish
WORKDIR /src/PlantBiologyEducation
RUN dotnet publish "PlantBiologyEducation.csproj" -c Release -o /app/publish

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "PlantBiologyEducation.dll"]
