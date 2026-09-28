# Dockerfile per applicazione ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
# Porta di default delle immagini ASP.NET Core da .NET 8 (ASPNETCORE_HTTP_PORTS=8080)
EXPOSE 8080

# Copia i file pubblicati dalla build
COPY . .

ENTRYPOINT ["dotnet", "jjodel-persistence.dll"]
