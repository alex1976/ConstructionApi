FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ConstructionApi.csproj ./
RUN dotnet restore ConstructionApi.csproj

COPY Program.cs ./
COPY appsettings*.json ./
COPY Controllers/ ./Controllers/
COPY Data/ ./Data/
COPY Models/ ./Models/
COPY Migrations/ ./Migrations/
COPY Samples/ ./Samples/

RUN dotnet publish ConstructionApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ConstructionApi.dll"]
