# User/Credential Separation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Separate user identity from authentication credentials to prepare for Google OAuth integration.

**Architecture:** Split the current `User` entity (which mixes identity and auth) into `User` (identity: id, name, email) and `UserCredential` (auth: provider, password_hash, external_subject). New `user_credential` table with 1 to N relationship to `user`. Use cases that create/update both entities wrap operations in a transaction via `ITransactionManager` abstraction (adapter over EF Core transactions, per global rules about abstracting external libs).

**Tech Stack:** .NET 9, EF Core 9 (SQL Server), xUnit, Moq, Argon2 password hashing.

---

## File Structure

**New files:**
- `Idiomas.Core/Domain/Enum/AuthProvider.cs` - Enum: Local, Google
- `Idiomas.Core/Domain/Entity/UserCredential.cs` - Auth credential entity
- `Idiomas.Core/Infrastructure/Database/Model/UserCredentialModel.cs` - EF Core model for user_credential table
- `Idiomas.Core/Infrastructure/Database/Mapper/UserCredentialMappingExtension.cs` - Model to Entity mapper
- `Idiomas.Core/Interface/Repository/IUserCredentialRepository.cs` - Credential repository interface
- `Idiomas.Core/Infrastructure/Database/Repository/UserCredentialRepository.cs` - Credential repository implementation
- `Idiomas.Core/Interface/Service/IDatabaseTransaction.cs` - Transaction handle interface
- `Idiomas.Core/Interface/Service/ITransactionManager.cs` - Transaction abstraction interface
- `Idiomas.Core/Infrastructure/Service/Transaction/EfCoreTransactionManager.cs` - EF Core transaction adapter
- `Idiomas.Tests.Core/Domain/Entity/UserCredentialTest.cs` - Entity unit test
- `Idiomas.Tests.Core/Infrastructure/Database/Repository/UserCredentialRepositoryTest.cs` - Repository integration test
- `Idiomas.Tests.Core/Infrastructure/Database/Mapper/UserCredentialMappingExtensionTest.cs` - Mapper test
- `Idiomas.Tests.Core/Application/Mapper/UserMappingExtensionTest.cs` - Application mapper test
- `Idiomas.Core/Infrastructure/Database/Migrations/<timestamp>_CreateUserCredentialTable.cs` - Migration

**Modified files:**
- `Idiomas.Core/Domain/Entity/User.cs` - Remove Password field
- `Idiomas.Core/Infrastructure/Database/Model/UserModel.cs` - Remove Password column
- `Idiomas.Core/Infrastructure/Database/Context/ApplicationContext.cs` - Add UserCredential DbSet + indexes
- `Idiomas.Core/Infrastructure/Database/Mapper/UserMappingExtension.cs` - Remove password from mappings
- `Idiomas.Core/Infrastructure/Database/Repository/UserRepository.cs` - Remove password from Update
- `Idiomas.Core/Infrastructure/Database/DependencyInjection.cs` - Register IUserCredentialRepository + ITransactionManager
- `Idiomas.Core/Infrastructure/Service/DependencyInjection.cs` - Register ITransactionManager
- `Idiomas.Core/Application/Mapper/UserMappingExtension.cs` - Remove password from ToEntity, add ToCredentialEntity
- `Idiomas.Core/Application/DTO/User.cs` - UpdateUserDTO.Password nullable
- `Idiomas.Core/Application/UseCase/UserCase/CreateUser.cs` - Create user + credential in transaction
- `Idiomas.Core/Application/UseCase/UserCase/UpdateUser.cs` - Update profile + optional credential in transaction
- `Idiomas.Core/Application/UseCase/AuthCase/MailPasswordLogin.cs` - Verify password via credential
- `Idiomas.Core/Application/UseCase/AuthCase/ResetPassword.cs` - Update credential password
- `Idiomas.Core/Presentation/Http/Validator/User/UpdateUserValidator.cs` - Conditional password validation
- `Idiomas.Tests.Core/Application/UseCase/UserCase/UserCaseTest.cs` - Update CreateUserTest
- `Idiomas.Tests.Core/Application/UseCase/Auth/MailPasswordLoginTest.cs` - Update for credential
- `Idiomas.Tests.Core/Application/UseCase/AuthCase/ResetPasswordTest.cs` - Update for credential
- `Idiomas.Tests.Core/Application/UseCase/AuthCase/ForgotPasswordTest.cs` - Update User constructor (3 args)

---

