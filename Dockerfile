FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build

# Copy project files and Directory.Build.targets for restore
COPY endpoint/Directory.Build.targets ./Directory.Build.targets
COPY endpoint/version.json ./version.json
COPY endpoint/src/Viae.ActivityPub/Viae.ActivityPub.csproj ./src/Viae.ActivityPub/
COPY endpoint/src/Viae.Domain/Viae.Domain.csproj ./src/Viae.Domain/
COPY endpoint/src/Viae.Persistence/Viae.Persistence.csproj ./src/Viae.Persistence/
COPY endpoint/src/Viae.Services/Viae.Services.csproj ./src/Viae.Services/
COPY endpoint/src/Viae.Presentation/Viae.Presentation.csproj ./src/Viae.Presentation/

# Restore dependencies (downloads packages to Docker cache)
RUN dotnet restore ./src/Viae.Presentation

# Copy all source files (including .git for versioning)
COPY endpoint/ ./
COPY .git/ ./.git/

# Publish (will use cached packages from earlier restore, only re-evaluating project graph)
RUN dotnet publish ./src/Viae.Presentation -c Release -o /build/publish \
    -p:SatelliteResourceLanguages=en \
    -p:DebugType=none \
    -p:DebugSymbols=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /build/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Viae.Presentation.dll"]