# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["RentABike.sln", "./"]
COPY ["src/RentABike.API/RentABike.API.csproj", "src/RentABike.API/"]
COPY ["src/RentABike.Application/RentABike.Application.csproj", "src/RentABike.Application/"]
COPY ["src/RentABike.Domain/RentABike.Domain.csproj", "src/RentABike.Domain/"]
COPY ["src/RentABike.Infrastructure/RentABike.Infrastructure.csproj", "src/RentABike.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "RentABike.sln"

# Copy all source files
COPY src/ ./src/

# Build and publish
WORKDIR "/src/src/RentABike.API"
RUN dotnet build "RentABike.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RentABike.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create directory for uploads
RUN mkdir -p /app/wwwroot/uploads

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 8080

# Set entry point
ENTRYPOINT ["dotnet", "RentABike.API.dll"]

