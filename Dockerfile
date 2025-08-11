# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY IdiomasAPI.csproj ./
RUN dotnet restore IdiomasAPI.csproj

COPY . .
RUN dotnet publish IdiomasAPI.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "IdiomasAPI.dll"]
EXPOSE 5076