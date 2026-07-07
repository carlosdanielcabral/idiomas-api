# 🌍 Idiomas API

> 🏗️ **Under development** — A comprehensive REST API for a language learning platform.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-🐳-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)

**Idiomas API** is a robust backend for language learning applications, providing everything needed to build interactive and personalized learning experiences — from secure user authentication and AI-powered conversations to vocabulary dictionaries and file storage.

---

## ✨ Features

- 🔐 **User Authentication** — JWT-based authentication with secure password hashing using Argon2
- 🤖 **AI Conversations** — Integration with Google Gemini AI for interactive language learning conversations
- 📚 **Dictionary Management** — Create and manage vocabulary dictionaries
- ☁️ **File Storage** — Azure Blob Storage integration for file uploads and management
- 🛡️ **Rate Limiting** — Built-in rate limiting to protect against abuse (100 requests per minute per IP)
- 📘 **API Documentation** — Interactive Swagger UI for API exploration

---

## 🚀 Technologies

| Category | Technology |
|----------|------------|
| Framework | **.NET 9.0** — Modern ASP.NET Core Web API |
| ORM | **Entity Framework Core 9.0.8** |
| Database | **SQL Server** (via Docker container) |
| Storage | **Azure Blob Storage** |
| Authentication | **JWT** tokens with **Argon2** hashing |
| Documentation | **Swashbuckle / Swagger** |
| Containerization | **Docker & Docker Compose** |
| AI | **Google Gemini AI** |

---

## 📋 Prerequisites

Before you begin, make sure you have the following installed and configured:

- 🐳 [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/)
- 🛠️ [.NET 9.0 SDK](https://dotnet.microsoft.com/) (for local development without Docker)
- ☁️ Azure Storage account (for file storage features)
- 🔑 Gemini API key (for AI conversation features)

---

## 🏁 Getting Started

### 🐳 Using Docker Compose (Recommended)

The fastest way to get the project running locally is with Docker Compose.

#### 1. Clone the repository

```bash
git clone <repository-url>
cd IdiomasAPI
```

#### 2. Configure environment variables

Copy the example environment file and fill in your actual values:

```bash
cp .env.example .env
```

Edit `.env` with the following:

- Database credentials
- JWT configuration (`Key`, `Issuer`, `Audience`)
- Azure Storage configuration
- Gemini API key
- Encryption key (minimum 32 characters)

#### 3. Start the application

```bash
docker-compose up -d
```

This will start three services:

| Service | Description |
|---------|-------------|
| `api` | The main API application |
| `database` | SQL Server database |
| `migration` | Database migration service |

#### 4. Verify the application

The API will be available at:

```
http://localhost:5076
```

---

### 💻 Running Locally (Without Docker)

If you prefer to run the application directly on your machine:

#### 1. Install dependencies

```bash
cd Idiomas.Core
dotnet restore
```

#### 2. Configure environment variables

Create a `.env` file in the `IdiomasAPI` directory with the required variables. See `.env.example` for reference.

#### 3. Run the application

```bash
dotnet run
```

---

## 📖 API Documentation

Once the application is running, you can explore the interactive API documentation at:

🔗 **Swagger UI**: [http://localhost:5076/swagger](http://localhost:5076/swagger)

The Swagger UI provides:

- Complete API endpoint documentation
- Request/response schemas
- Try-it-out functionality for testing endpoints

---

## 🔌 Ports

| Service | Port |
|---------|------|
| API | `5076` |
| Database | `1433` |

---

## 🔧 Environment Variables

Key environment variables (see `.env.example` for the complete list):

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string |
| `MSSQL_SA_PASSWORD` | SQL Server SA password |
| `API_URL` | API binding URL |
| `Jwt__Key` | JWT signing key |
| `Jwt__Issuer` | JWT issuer |
| `Jwt__Audience` | JWT audience |
| `Azure__Storage__AccountName` | Azure Storage account name |
| `Azure__Storage__BlobServiceUri` | Azure Blob Storage URI |
| `Azure__Storage__ContainerName` | Azure container name |
| `Gemini__ApiKey` | Gemini AI API key |
| `Gemini__Model` | Gemini AI model name |
| `Encryption__Key` | Encryption key (minimum 32 characters) |
| `FrontendLocalUrl` | Allowed frontend URLs for CORS |

---

## 🏗️ Project Structure

The application follows **Clean Architecture** principles:

```
Idiomas.Core/
├── Application/       # Application logic and use cases
├── Domain/          # Domain entities and interfaces
├── Helper/          # Helper utilities and services
├── Infrastructure/  # External dependencies (database, storage, AI)
└── Presentation/    # API layer (controllers, routes, DTOs)
```

---

## 🗄️ Database Migrations

When using Docker Compose, migrations run automatically via the `migration` service.

### Run migrations manually

```bash
cd Idiomas.Core
dotnet ef database update --context ApplicationContext
```

### Create a new migration

```bash
dotnet ef migrations add <MigrationName> --context ApplicationContext
```

---

## 🧪 Running Tests

```bash
cd Idiomas.Tests.Core
dotnet test
```

---


