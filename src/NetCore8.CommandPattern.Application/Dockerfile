# src/NetCore8.CommandPattern.Application/Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NetCore8.CommandPattern.Application/NetCore8.CommandPattern.Application.csproj", "src/NetCore8.CommandPattern.Application/"]
COPY ["src/NetCore8.CommandPattern.Core/NetCore8.CommandPattern.Core.csproj", "src/NetCore8.CommandPattern.Core/"]
COPY ["src/NetCore8.CommandPattern.Infrastructure/NetCore8.CommandPattern.Infrastructure.csproj", "src/NetCore8.CommandPattern.Infrastructure/"]
RUN dotnet restore "./src/NetCore8.CommandPattern.Application/NetCore8.CommandPattern.Application.csproj"
COPY . .
WORKDIR "/src/src/NetCore8.CommandPattern.Application"
RUN dotnet build "./NetCore8.CommandPattern.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NetCore8.CommandPattern.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
USER root
RUN mkdir -p /app/keys && chown -R app:app /app/keys
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetCore8.CommandPattern.Application.dll"]