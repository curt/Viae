FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build

COPY endpoint/ ./
RUN dotnet publish ./src/Viae.Presentation -c Release -o /build/publish -p:SatelliteResourceLanguages=en

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /build/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Viae.Presentation.dll"]