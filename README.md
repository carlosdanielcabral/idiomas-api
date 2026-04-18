# Idiomas API

A comprehensive REST API for a language learning platform that provides authentication, AI-powered conversations, dictionary management, and file storage capabilities.

## Features

- **User Authentication**: JWT-based authentication with secure password hashing using Argon2
- **AI Conversations**: Integration with Google Gemini AI for interactive language learning conversations
- **Dictionary Management**: Create and manage vocabulary dictionaries
- **File Storage**: Azure Blob Storage integration for file uploads and management
- **Rate Limiting**: Built-in rate limiting to protect against abuse (100 requests per minute per IP)
- **API Documentation**: Interactive Swagger UI for API exploration

## Technologies

- **.NET 9.0**: Modern ASP.NET Core Web API framework
- **Entity Framework Core 9.0.8**: ORM for database operations
- **SQL Server**: Primary database (via Docker container)
- **Azure Blob Storage**: Cloud file storage solution
- **JWT Authentication**: Token-based authentication system
- **Argon2**: Secure password hashing algorithm
- **Swashbuckle/Swagger**: API documentation generation
- **Docker & Docker Compose**: Containerization and orchestration
- **Gemini AI**: Google's AI model for conversation features

## Prerequisites

- Docker and Docker Compose installed
- .NET 9.0 SDK (for local development without Docker)
- Azure Storage account (for file storage features)
- Gemini API key (for AI conversation features)

## Getting Started

### Using Docker Compose (Recommended)

1. **Clone the repository**

```bash
git clone <repository-url>
cd IdiomasAPI
```

2. **Configure environment variables**

Copy the example environment file and configure it:

```bash
cp .env.example .env
```

Edit `.env` with your actual values:
- Database credentials
- JWT configuration (Key, Issuer, Audience)
- Azure Storage configuration
- Gemini API key
- Encryption key (minimum 32 characters)

3. **Start the application**

```bash
docker-compose up -d
```

This will start three services:
- **api**: The main API application
- **database**: SQL Server database
- **migration**: Database migration service

4. **Verify the application**

The API will be available at `http://localhost:5076`

### Running Locally (Without Docker)

1. **Install dependencies**

```bash
cd Idiomas.Core
dotnet restore
```

2. **Configure environment variables**

Create a `.env` file in the `IdiomasAPI` directory with the required variables (see `.env.example`)

3. **Run the application**

```bash
dotnet run
```

## API Documentation

Once the application is running, access the interactive API documentation at:

**Swagger UI**: `http://localhost:5076/swagger`

The Swagger UI provides:
- Complete API endpoint documentation
- Request/response schemas
- Try-it-out functionality for testing endpoints

## Ports

- **API**: `5076`
- **Database**: `1433`

## Environment Variables

Key environment variables (see `.env.example` for complete list):

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
| `Encryption__Key` | Encryption key (min 32 characters) |
| `FrontendLocalUrl` | Allowed frontend URLs for CORS |

## Project Structure

The application follows Clean Architecture principles:

```
Idiomas.Core/
├── Application/       # Application logic and use cases
├── Domain/          # Domain entities and interfaces
├── Helper/          # Helper utilities and services
├── Infrastructure/  # External dependencies (database, storage, AI)
└── Presentation/    # API layer (controllers, routes, DTOs)
```

## Database Migrations

When using Docker Compose, migrations run automatically via the migration service.

To run migrations manually:

```bash
cd Idiomas.Core
dotnet ef database update --context ApplicationContext
```

To create a new migration:

```bash
dotnet ef migrations add <MigrationName> --context ApplicationContext
```

## Development

### Running Tests

```bash
cd Idiomas.Tests.Core
dotnet test
```


## Contact

- **Developer**: Carlos Daniel Cabral
- **Email**: dev.carlosdaniel@gmail.com
