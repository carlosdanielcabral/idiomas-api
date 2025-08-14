# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
COPY Idiomas.csproj ./
RUN dotnet restore Idiomas.csproj
COPY . .
RUN dotnet publish Idiomas.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Idiomas.dll"]
EXPOSE 5076