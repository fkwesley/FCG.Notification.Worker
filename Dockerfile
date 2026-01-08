# ============================
# Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas o csproj (cache de restore)
COPY FCG.Notification.Worker.csproj ./
RUN dotnet restore FCG.Notification.Worker.csproj

# Copia o restante do código
COPY . .
RUN dotnet publish FCG.Notification.Worker.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ============================
# Runtime stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

# Install support to globalization
RUN apk add --no-cache icu-libs tzdata

# Set .NET to use ICU
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ=America/Sao_Paulo

WORKDIR /app

ENV DOTNET_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FCG.Notification.Worker.dll"]
