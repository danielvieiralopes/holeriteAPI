# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia o csproj e restaura dependências
COPY HoleriteApi/*.csproj ./HoleriteApi/
RUN dotnet restore ./HoleriteApi/HoleriteApi.csproj

# Copia tudo e compila
COPY . .
RUN dotnet publish ./HoleriteApi/HoleriteApi.csproj -c Release -o /out

# Etapa 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "HoleriteApi.dll"]
