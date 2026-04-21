FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY ["JuegoColores.csproj", "./"]
RUN dotnet restore "./JuegoColores.csproj"

# Copiar el resto del código y compilar
COPY . .
RUN dotnet publish "JuegoColores.csproj" -c Release -o /app/publish

# Generar la imagen final
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .

# Para Render, usamos la variable de entorno PORT
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

CMD ["dotnet", "JuegoColores.dll"]
