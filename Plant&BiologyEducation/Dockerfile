# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PlantBiologyEducation.csproj", "./"]
RUN dotnet restore "./PlantBiologyEducation.csproj"

COPY . .
RUN dotnet publish "./PlantBiologyEducation.csproj" -c Release -o /app/publish

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "PlantBiologyEducation.dll"]
