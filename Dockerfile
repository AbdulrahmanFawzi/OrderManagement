# Simple multi-stage Dockerfile for learning.
# Stage 1 (build): restore + publish the API.
# Stage 2 (runtime): run the published output on the ASP.NET runtime image.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first for better layer caching.
COPY OrderManagement.Api/OrderManagement.Api.csproj OrderManagement.Api/
COPY OrderManagement.Application/OrderManagement.Application.csproj OrderManagement.Application/
COPY OrderManagement.Domain/OrderManagement.Domain.csproj OrderManagement.Domain/
COPY OrderManagement.Infrastructure/OrderManagement.Infrastructure.csproj OrderManagement.Infrastructure/
RUN dotnet restore OrderManagement.Api/OrderManagement.Api.csproj

# Copy everything else and publish.
# No --no-restore here on purpose: publish runs an implicit restore,
# so the build self-heals even if obj/ files are ever stale.
COPY . .
RUN dotnet publish OrderManagement.Api/OrderManagement.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# The API listens on 8080 inside the container.
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OrderManagement.Api.dll"]
