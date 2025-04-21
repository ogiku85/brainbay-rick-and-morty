# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as separate layer
COPY ["src/Brainbay.RickAndMorty.ConsoleApp/Brainbay.RickAndMorty.ConsoleApp.csproj", "src/Brainbay.RickAndMorty.ConsoleApp/"]
RUN dotnet restore "src/Brainbay.RickAndMorty.ConsoleApp/Brainbay.RickAndMorty.ConsoleApp.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Brainbay.RickAndMorty.ConsoleApp"
RUN dotnet build "Brainbay.RickAndMorty.ConsoleApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Brainbay.RickAndMorty.ConsoleApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Brainbay.RickAndMorty.ConsoleApp.dll"]
