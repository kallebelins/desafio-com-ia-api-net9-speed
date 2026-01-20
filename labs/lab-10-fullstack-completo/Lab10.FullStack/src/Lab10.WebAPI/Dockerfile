# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["src/Lab10.Domain/Lab10.Domain.csproj", "Lab10.Domain/"]
COPY ["src/Lab10.Application/Lab10.Application.csproj", "Lab10.Application/"]
COPY ["src/Lab10.Infrastructure/Lab10.Infrastructure.csproj", "Lab10.Infrastructure/"]
COPY ["src/Lab10.WebAPI/Lab10.WebAPI.csproj", "Lab10.WebAPI/"]

RUN dotnet restore "Lab10.WebAPI/Lab10.WebAPI.csproj"

# Copy all source code
COPY src/ .

# Build
WORKDIR "/src/Lab10.WebAPI"
RUN dotnet build "Lab10.WebAPI.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Lab10.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Copy NLog config
COPY src/Lab10.WebAPI/NLog.config .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Entry point
ENTRYPOINT ["dotnet", "Lab10.WebAPI.dll"]
