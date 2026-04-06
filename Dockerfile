# Stage 1: Build Environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project specification first
COPY ["visitor_management.csproj", "./"]

# Restore dependencies
RUN dotnet restore "visitor_management.csproj"

# Copy remaining source code
COPY . .

# Build the project
RUN dotnet build "visitor_management.csproj" -c Release -o /app/build

# Publish the project for production
RUN dotnet publish "visitor_management.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime Environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose internal Render port configuration
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "visitor_management.dll"]
