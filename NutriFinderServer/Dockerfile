# this docker file was made using Rider dockerfile templete

# ============================
# Base image for runtime only
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Set working directory in container
WORKDIR /app

# Expose ports (e.g. 8080 for HTTP, 8081 for gRPC or admin endpoints)
EXPOSE 8080
EXPOSE 8081


# ==============================
# Build stage using full SDK
# ==============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Allow overriding build configuration
ARG BUILD_CONFIGURATION=Release

# Set working directory for build environment
WORKDIR /src

# Copy project file and restore dependencies (enables build caching)
COPY ["NutriFinderServer/NutriFinderServer.csproj", "NutriFinderServer/"]
RUN dotnet restore "NutriFinderServer/NutriFinderServer.csproj"

# Copy entire source tree and build project
COPY . .
WORKDIR "/src/NutriFinderServer"
RUN dotnet build "./NutriFinderServer.csproj" -c $BUILD_CONFIGURATION -o /app/build


# =====================================
# Publish stage: Optimizes output for deployment
# =====================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NutriFinderServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# ===============================
# Final runtime image deployment
# ===============================
FROM base AS final

# Set working directory in runtime container
WORKDIR /app

# Copy published app from previous stage
COPY --from=publish /app/publish .

# Define entry point for container
ENTRYPOINT ["dotnet", "NutriFinderServer.dll"]
