# this docker file was made using Rider dockerfile templete

# ============================
# Base image for runtime only
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Set working directory in container
WORKDIR /app


# ==============================
# Build stage using full SDK
# ==============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Allow overriding build configuration
ARG BUILD_CONFIGURATION=Release

# Set working directory for build environment
WORKDIR /src

# Restore dependencies (separate step to leverage caching)
COPY ["NutriFinderClient/NutriFinderClient.csproj", "NutriFinderClient/"]
RUN dotnet restore "NutriFinderClient/NutriFinderClient.csproj"

# Copy remaining source code and build project
COPY . .
WORKDIR "/src/NutriFinderClient"
RUN dotnet build "./NutriFinderClient.csproj" -c $BUILD_CONFIGURATION -o /app/build


# ===============================================
# Publish stage: Optimizes output for deployment
# ===============================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NutriFinderClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# ===============================================
# Final stage: Runtime image with published output
# ===============================================
FROM base AS final

# Set working directory in runtime container
WORKDIR /app

# Copy published app from previous stage
COPY --from=publish /app/publish .

# Define container entry point
ENTRYPOINT ["dotnet", "NutriFinderClient.dll"]
