FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/CoordService/CoordService.csproj src/CoordService/
RUN dotnet restore src/CoordService/CoordService.csproj
COPY . .
WORKDIR /src/src/CoordService
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoordService.dll"]
