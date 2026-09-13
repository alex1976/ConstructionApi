FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ConstructionApi.McpServer.csproj ./
RUN dotnet restore ConstructionApi.McpServer.csproj

COPY . .
RUN dotnet publish ConstructionApi.McpServer.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ConstructionApi.McpServer.dll"]
