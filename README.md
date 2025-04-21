# Rick and Morty Character App

This solution is a full-stack .NET 8 project that interacts with the [Rick and Morty API](https://rickandmortyapi.com/), focusing on alive characters and their origins. It consists of two main components:

## 🧠 Console App – `RickAndMortyImporter`
A background service that:
- Fetches all **Alive** characters from the public Rick and Morty API
- Normalizes the data into SQL entities (Characters, Origins, Episodes)
- Stores the data in a SQL Server database using EF Core
- Skips dead characters
- Can be run on-demand to sync latest data

## 🌐 Web App – `RickAndMorty.WebApp`
An ASP.NET Core MVC app that:
- Displays all characters from the database
- Allows adding new characters manually
- Supports filtering characters by planet
- Uses redis caching for performance
- Includes custom headers to show data source (e.g. `from-database`)
- Returns proper HTTP response codes and uses `ProblemDetails` for errors

## 🧪 Testing
- **Unit tests** for all core services and controllers
- **Integration tests** using Testcontainers to spin up SQL Server in Docker for validating end-to-end behavior

## 🐳 Docker Support
- The solution can be run locally using Docker Compose
- Includes SQL Server, the importer, and web app services

---

### Run Locally
```bash
docker-compose up --build
