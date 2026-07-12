# 🌍 Idiomas API

> 🏗️ **Under development** — A comprehensive REST API for a language learning platform.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-🐳-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)

**Idiomas API** is a robust backend for language learning applications, providing everything needed to build interactive and personalized learning experiences — from secure user authentication and AI-powered conversations to vocabulary dictionaries and file storage.

## 📑 Table of Contents

- [Features](#-features)
- [Technologies](#-technologies)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [API Documentation](#-api-documentation)
- [Ports](#-ports)
- [Email & Mailpit](#-email--mailpit)
- [Environment Variables](#-environment-variables)
- [Project Structure](#️-project-structure)
- [Exception Handling](#️-exception-handling)
- [Database Migrations](#️-database-migrations)
- [Running Tests](#-running-tests)

---

## ✨ Features

- 🔐 **User Authentication** — JWT-based authentication with secure password hashing using Argon2, plus **Google OAuth login** (Sign in with Google) alongside traditional email/password login
- 🔄 **Unit of Work** — Transactional consistency across repositories via a generic `IUnitOfWork` abstraction (EF Core implementation), used by use cases that touch multiple aggregates in a single operation (e.g. account creation, linking a Google credential)
- 🤖 **AI Conversations** — Integration with Google Gemini AI for interactive language learning conversations
- 📚 **Dictionary Management** — Create and manage vocabulary dictionaries
- ☁️ **File Storage** — Azure Blob Storage integration for file uploads and management
- 🛡️ **Rate Limiting** — Built-in rate limiting to protect against abuse (100 requests per minute per IP)
- 📧 **Email Service** — Send email notifications through a generic `IEmailService` interface. **SendGrid** is used in production, while **Mailpit** is used as a local SMTP server in development so emails can be inspected without sending real messages
- 📖 **API Documentation** — Interactive Swagger UI for API exploration

---

## 🚀 Technologies

| Category | Technology |
|----------|------------|
| Framework | **.NET 9.0** — Modern ASP.NET Core Web API |
| ORM | **Entity Framework Core 9.0.8** |
| Database | **SQL Server** (via Docker container) |
| Storage | **Azure Blob Storage** |
| Authentication | **JWT** tokens with **Argon2** hashing, plus **Google OAuth** (`Google.Apis.Auth`) |
| Transactions | **Unit of Work** pattern on top of EF Core (`IUnitOfWork` / `EfCoreUnitOfWork`) |
| Email | **SendGrid** (production) and **Mailpit** via SMTP/MailKit (development) |
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
- 🔑 Google OAuth Client ID (for "Sign in with Google")
- 🔑 SendGrid API key (only required in production; local development uses Mailpit instead, see [Email & Mailpit](#-email--mailpit))

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
- Google OAuth Client ID
- Gemini API key
- Encryption key (minimum 32 characters)
- SendGrid API key (optional in development, since Mailpit is used instead — see [Email & Mailpit](#-email--mailpit))

#### 3. Start the application

```bash
docker-compose up -d
```

This will start four services:

| Service | Description |
|---------|-------------|
| `api` | The main API application |
| `database` | SQL Server database |
| `migration` | Database migration service |
| `mailpit` | Local SMTP server used to send and inspect emails in development (see [Email & Mailpit](#-email--mailpit)) |

#### 4. Verify the application

The API will be available at:

```
http://localhost:5076
```

The Mailpit web UI, used to inspect emails sent by the application, will be available at:

```
http://localhost:8025
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
| Mailpit (SMTP) | `1025` |
| Mailpit (Web UI) | `8025` |

---

## 📧 Email & Mailpit

Emails are sent through the generic `IEmailService` interface, which has two implementations:

| Environment | Implementation | Notes |
|-------------|-----------------|-------|
| Development | `SmtpEmailService` (MailKit) | Sends emails to the local **Mailpit** SMTP server, so no real email provider is needed |
| Other environments | `SendGridClientService` | Sends emails through **SendGrid**, requires `SendGrid__ApiKey` |

The implementation is selected automatically at startup based on `ASPNETCORE_ENVIRONMENT` — no manual configuration is needed to switch between them.

When running via Docker Compose, the `mailpit` service is started automatically. To see the emails sent by the application, open the Mailpit web UI:

🔗 **Mailpit UI**: [http://localhost:8025](http://localhost:8025)

If you run the API locally without Docker, you can start Mailpit on its own:

```bash
docker run -d --name mailpit -p 1025:1025 -p 8025:8025 axllent/mailpit:v1.22
```

The `Smtp:Host` / `Smtp:Port` configuration in `appsettings.Development.json` already points to `localhost:1025`, matching this setup.

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
| `Google__ClientId` | Google OAuth Client ID used to validate "Sign in with Google" tokens |
| `Gemini__ApiKey` | Gemini AI API key |
| `Gemini__Model` | Gemini AI model name |
| `Encryption__Key` | Encryption key (minimum 32 characters) |
| `SendGrid__ApiKey` | SendGrid API key for sending emails (production only) |
| `Email__SenderAddress` | Default email sender address |
| `Email__SenderName` | Default email sender name |
| `Smtp__Host` | SMTP host used in development (Mailpit) |
| `Smtp__Port` | SMTP port used in development (Mailpit) |
| `FrontendLocalUrl` | Allowed frontend URLs for CORS |

---

## 🏗️ Project Structure

The application follows **Clean Architecture** principles:

```
Idiomas.Core/
├── Application/       # Application logic and use cases
├── Domain/            # Domain entities and interfaces
├── Exceptions/        # Base exception class and shared validation exceptions
├── Helper/            # Helper utilities and services
├── Infrastructure/    # External dependencies (database, storage, AI)
└── Presentation/      # API layer (controllers, routes, DTOs)
```

---

## ⚠️ Exception Handling

All API exceptions inherit from `ApiException` (located in the `Exceptions` layer), which carries an `ErrorCode`, `Title`, `HttpStatusCode`, `Detail`, and optional `Extensions`. The `ApiExceptionMiddleware` in the Presentation layer catches these and converts them into RFC 7807 `ProblemDetails` JSON responses.

### Exception organization

Exceptions are organized by the layer that throws them:

| Location | Namespace | Purpose |
|----------|-----------|---------|
| `Exceptions/` | `Idiomas.Core.Exceptions` | Base `ApiException` class |
| `Exceptions/Validation/` | `Idiomas.Core.Exceptions.Validation` | Generic validation exceptions (e.g. `FieldRequiredException`, `StringTooShortException`) shared across layers |
| `Application/Exceptions/` | `Idiomas.Core.Application.Exceptions.*` | Business-rule exceptions (Auth, Conversation, Dictionary, File, User) thrown by use cases |
| `Infrastructure/Exceptions/` | `Idiomas.Core.Infrastructure.Exceptions.*` | Infrastructure failures (Email, Google, LLM) thrown by external service adapters |
| `Helper/Exceptions/` | `Idiomas.Core.Helper.Exceptions` | Helper-specific exceptions (e.g. `LanguageInvalidException`, `LanguageRequiredException`) |
| `Presentation/Http/Middleware/` | `Idiomas.Core.Presentation.Http.Middleware` | `ApiExceptionMiddleware` and `ProblemDetailsUris` for mapping exceptions to HTTP responses |

### Example response

When an exception is thrown, the API returns an RFC 7807 `ProblemDetails` JSON response:

```json
{
  "type": "tag:idiomas.api,2026:error:validation:string-too-short",
  "title": "String too short",
  "status": 400,
  "detail": "The field 'password' must be at least 8 characters long.",
  "instance": "tag:idiomas.api,2026:trace:00-abc123def456-..."
}
```

| Field | Description |
|-------|-------------|
| `type` | Unique error identifier (tag URI based on `ErrorCode`) |
| `title` | Short human-readable summary |
| `status` | HTTP status code |
| `detail` | Specific detail about what went wrong |
| `instance` | Request trace identifier for debugging. This is a tag URI derived from the ASP.NET Core `TraceIdentifier`, which uniquely identifies the request within the server pipeline. It can be used to correlate a specific error with server logs when investigating issues reported by clients |

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


