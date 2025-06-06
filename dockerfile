# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore ./HoleriteApi.csproj

# Copia tudo e compila
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app


COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HoleriteApi.dll"]
