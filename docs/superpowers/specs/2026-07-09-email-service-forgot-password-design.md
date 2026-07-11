# Email Service & Forgot Password — Design

> Date: 2026-07-09
> Status: Approved for implementation planning

## 1. Context

IdiomasAPI is a .NET 9 REST backend for language learning. Authentication currently supports email/password login via `MailPasswordLogin` use case. This document specifies two additions:

1. **Email Service** — infrastructure service for sending transactional emails via SendGrid
2. **Forgot Password flow** — first consumer of the email service, covering token generation and password reset

## 2. Email Service

### 2.1 Contract

New interface at `Interface/Service/IEmailService.cs`:

```csharp
public interface IEmailService
{
    Task SendAsync(EmailMessage message);
}

public record EmailMessage(
    string To,
    string Subject,
    string HtmlBody
);
```

### 2.2 Implementation

`Infrastructure/Service/Email/SendGridEmailService.cs` implements `IEmailService`:

- Receives `IConfiguration` to read `SendGrid:ApiKey`, `Email:SenderAddress`, `Email:SenderName`
- Uses the official `SendGrid` NuGet package
- Throws a new `EmailDeliveryException` (extends `ApiException`) when SendGrid returns a non-2xx response
- All three config values are required; missing values throw `InvalidOperationException` at startup

### 2.3 Template Loading

`Infrastructure/Service/Email/EmailTemplateLoader.cs`:

- Loads `.html` files from `Templates/Email/` folder (relative to the assembly)
- Replaces placeholders in the format `{{VariableName}}` using `string.Replace`
- Throws `FileNotFoundException` if the template file does not exist
- Use cases call `EmailTemplateLoader` to build the `HtmlBody` before passing it to `IEmailService`

### 2.4 Templates

HTML files in `Templates/Email/`:

- `PasswordResetEmail.html` — contains `{{UserName}}` and `{{ResetLink}}` placeholders

### 2.5 Configuration

Environment variables (following the existing `__` convention):

```
SendGrid__ApiKey=your_sendgrid_api_key_here
Email__SenderAddress=noreply@idiomas.app
Email__SenderName=Idiomas
```

These are added to `.env.example` for documentation. No sensitive values enter `appsettings.json`.

### 2.6 Dependency Injection

`Infrastructure/Service/DependencyInjection.cs` — add:

```csharp
services.AddScoped<IEmailService, SendGridEmailService>();
services.AddScoped<EmailTemplateLoader>();
```

## 3. Forgot Password Flow

### 3.1 Domain Entity

New entity `Domain/Entity/PasswordResetToken.cs`:

```
Id          Guid
UserId      Guid (FK → User)
Token       string (64-char hex, unique)
ExpiresAt   DateTime (UTC, now + 1 hour)
UsedAt      DateTime? (null = unused)
```

### 3.2 Repository

Interface `Interface/Repository/IPasswordResetTokenRepository.cs`:

```csharp
public interface IPasswordResetTokenRepository
{
    Task Insert(PasswordResetToken token);
    Task<PasswordResetToken?> GetByToken(string token);
    Task<PasswordResetToken?> GetActiveTokenByUserId(Guid userId);
    Task MarkAsUsed(PasswordResetToken token);
}
```

Implementation `Infrastructure/Database/Repository/PasswordResetTokenRepository.cs` uses the existing `ApplicationContext` (EF Core).

New migration adds table `PasswordResetTokens`.

### 3.3 Application DTOs

`Application/DTO/Auth.cs` — add:

```csharp
public record ForgotPasswordDTO(string Email);
public record ResetPasswordDTO(string Token, string NewPassword);
```

### 3.4 Use Cases

#### `ForgotPassword` (`Application/UseCase/AuthCase/ForgotPassword.cs`)

Dependencies: `IUserRepository`, `IPasswordResetTokenRepository`, `IEmailService`, `EmailTemplateLoader`, `IConfiguration`

```
Execute(ForgotPasswordDTO dto):
  1. user = userRepository.GetByEmail(dto.Email)
  2. if user == null → return (silent 200, no email sent)
  3. activeToken = tokenRepository.GetActiveTokenByUserId(user.Id)
  4. if activeToken != null → throw ApiException("Já existe uma solicitação ativa", 409)
  5. token = new PasswordResetToken { Token = GenerateSecureToken(), CreatedAt = now, ExpiresAt = now + 1h }
  6. tokenRepository.Insert(token)
  7. htmlBody = templateLoader.Load("PasswordResetEmail.html", { UserName, ResetLink })
  8. emailService.SendAsync(new EmailMessage(user.Email, "Redefinição de senha", htmlBody))
```

`GenerateSecureToken()` — private static method using `RandomNumberGenerator.GetHexString(64)`.

The reset link is built from `configuration["FrontendUrl"]` + `/reset-password?token=<token>`. The existing `FrontendLocalUrl` env var pattern is used for reference; a new `FrontendUrl` variable is added to `.env.example`.

#### `ResetPassword` (`Application/UseCase/AuthCase/ResetPassword.cs`)

Dependencies: `IPasswordResetTokenRepository`, `IUserRepository`, `IHash`

```
Execute(ResetPasswordDTO dto):
  1. token = tokenRepository.GetByTokenAsync(dto.Token)
  2. if token == null || token.ExpiresAt < now || token.UsedAt != null
       → throw ApiException("Token inválido ou expirado", 400)
  3. user = userRepository.GetById(token.UserId.ToString())
  4. user.Password = hash.Hash(dto.NewPassword)
  5. userRepository.Update(user)
  6. tokenRepository.MarkAsUsedAsync(token)
```

### 3.5 Presentation Layer

#### Validators

`Presentation/Http/Validator/Auth/ForgotPasswordValidator.cs`:
- `Email` required and matches email regex

`Presentation/Http/Validator/Auth/ResetPasswordValidator.cs`:
- `Token` required, non-empty
- `NewPassword` required, minimum 8 characters

#### Controller

`IAuthController.cs` — add two method signatures:

```csharp
Task<IResult> ForgotPassword(ForgotPasswordDTO dto, ForgotPassword useCase);
Task<IResult> ResetPassword(ResetPasswordDTO dto, ResetPassword useCase);
```

`AuthController.cs` — implement both:
- `ForgotPassword` → always returns `TypedResults.Ok()` (silent for non-existent emails)
- `ResetPassword` → returns `TypedResults.Ok()` on success

#### Routes

`AuthRoute.cs` — add two endpoints:

```
POST /auth/forgot-password   → 200 | 409 (active token already exists)
POST /auth/reset-password    → 200 | 400
```

Both use `.WithValidation<>()` following the existing pattern.

## 4. New Components Summary

| Component | Type | Path |
|---|---|---|
| `IEmailService` | Interface | `Interface/Service/` |
| `EmailMessage` | Record | `Interface/Service/` |
| `SendGridEmailService` | Service | `Infrastructure/Service/Email/` |
| `EmailTemplateLoader` | Helper | `Infrastructure/Service/Email/` |
| `PasswordResetEmail.html` | Template | `Templates/Email/` |
| `PasswordResetToken` | Entity | `Domain/Entity/` |
| `IPasswordResetTokenRepository` | Interface | `Interface/Repository/` |
| `PasswordResetTokenRepository` | Repository | `Infrastructure/Database/Repository/` |
| Migration | EF Core | `Infrastructure/Database/Migrations/` |
| `ForgotPasswordDTO` | DTO | `Application/DTO/Auth.cs` |
| `ResetPasswordDTO` | DTO | `Application/DTO/Auth.cs` |
| `ForgotPassword` | Use Case | `Application/UseCase/AuthCase/` |
| `ResetPassword` | Use Case | `Application/UseCase/AuthCase/` |
| `ForgotPasswordValidator` | Validator | `Presentation/Http/Validator/Auth/` |
| `ResetPasswordValidator` | Validator | `Presentation/Http/Validator/Auth/` |

## 5. Security Decisions

- **User enumeration:** `POST /auth/forgot-password` returns 200 for non-existent emails (no email sent). Returns 409 only when the email exists and already has an active token — this is acceptable since it requires knowing the email exists first.
- **Token uniqueness:** Only one active token per user is allowed. If an active token exists, the API returns 409 Conflict instead of generating a new one.
- **Token expiration:** 1 hour (UTC), checked server-side on reset
- **Token storage:** Plain hex string in DB (not hashed) — acceptable given tokens are single-use, short-lived, and stored server-side. If hashing is desired in a future iteration, apply `IHash` before persistence and compare hashes on validation.
- **Reset link destination:** Frontend URL (`FrontendUrl` env var) — the API never serves the reset form directly

## 6. Out of Scope

- Email verification on account creation (separate future spec)
- Progress notification emails (separate future spec)
- Rate limiting on `POST /auth/forgot-password`
- Token hashing at rest
