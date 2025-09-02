# Dockerfile per applicazione ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copia i file pubblicati dalla build
COPY . .

ENTRYPOINT ["dotnet", "jjodel-persistence.dll"]
