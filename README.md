# 👽 Rick and Morty Character App

A full-stack **.NET 8** project that interacts with the [Rick and Morty API](https://rickandmortyapi.com/api/character/), focusing on **alive characters** and their **origins**.

This solution includes:

- A background **console importer**
- A user-friendly **web application**
- Full **test coverage**
- **Dockerized** setup for local development
- Integrated **ELK stack** for observability

---

## 🧠 Console App – `RickAndMortyImporter`

A background service that:

- 🚀 Fetches all **Alive** characters from the Rick and Morty API  
- 🧩 Normalizes data into SQL entities: `Characters`, `Origins`, and `Episodes`  
- 💾 Persists the data using **EF Core** with a **MySQL** database  
- 🧟 Skips dead characters  
- 🔁 Can be run on-demand to sync the latest data  

---

## 🌐 Web App – `RickAndMorty.WebApp`

An **ASP.NET Core MVC** app that:

- 👀 Displays all characters stored in the database  
- ➕ Allows manually adding new characters  
- 🔍 Supports **filtering** by planet  
- ⚡ Uses **Redis caching** for fast data retrieval  
- 🧾 Adds custom headers (e.g., `from-database`) to responses  
- 🚦 Returns proper HTTP status codes and uses `ProblemDetails` for error handling  

---

## 🧪 Testing

- ✅ **Unit tests** for core services and controllers  
- 🔁 **Integration tests** using **Testcontainers** to spin up MySQL in Docker  
- 🧪 Ensures end-to-end validation of business logic  

---

## 🛠️ Tech Stack

- **.NET 8**
- **ASP.NET Core MVC**
- **EF Core** with **MySQL Server**
- **Redis** (caching)
- **Testcontainers** (integration testing)
- **Moq**, **xUnit**, **FluentAssertions**
- **ELK Stack** – [Elasticsearch](https://www.elastic.co/elasticsearch/), [Logstash](https://www.elastic.co/logstash/), and [Kibana](https://www.elastic.co/kibana/) for logging, monitoring, and data visualization  

---

## 🐳 Docker Support

Run the full solution using **Docker Compose**, including:

- MySQL Server  
- Importer console app  
- ASP.NET Core Web App  

---

## 🚀 Run Locally

```bash
docker-compose up --build
