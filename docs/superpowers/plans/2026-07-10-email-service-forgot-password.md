# Email Service & Forgot Password Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a SendGrid-based email service and a forgot-password flow (token generation, email sending, password reset) to IdiomasAPI.

**Architecture:** Layered .NET 9 API following existing patterns. New `IEmailService` interface + `SendGridEmailService` implementation in Infrastructure/Service. New `PasswordResetToken` entity + repository + EF migration. Two new use cases (`ForgotPassword`, `ResetPassword`) in Application. Two new endpoints in Presentation. Templates as local `.html` files with `{{Placeholder}}` substitution.

**Tech Stack:** .NET 9, EF Core (SQL Server), SendGrid NuGet package, xUnit for tests.

**Spec:** `docs/superpowers/specs/2026-07-09-email-service-forgot-password-design.md`

---

## File Structure

**Create:**
- `Interface/Service/IEmailService.cs` — email service contract + `EmailMessage` record
- `Interface/Repository/IPasswordResetTokenRepository.cs` — token repository contract
- `Infrastructure/Service/Email/SendGridEmailService.cs` — SendGrid implementation
- `Infrastructure/Service/Email/EmailTemplateLoader.cs` — loads .html templates, replaces placeholders
- `Infrastructure/Service/Email/EmailTemplatePlaceholder.cs` — record for placeholder key/value
- `Infrastructure/Service/Email/ISendGridClient.cs` — adapter interface for SendGridClient
- `Infrastructure/Service/Email/SendGridClientAdapter.cs` — adapter implementation
- `Domain/Entity/PasswordResetToken.cs` — token entity
- `Infrastructure/Database/Model/PasswordResetTokenModel.cs` — EF model
- `Infrastructure/Database/Mapper/PasswordResetTokenMappingExtension.cs` — entity ↔ model mapping
- `Infrastructure/Database/Repository/PasswordResetTokenRepository.cs` — repository implementation
- `Infrastructure/Database/Migrations/<generated>_CreatePasswordResetTokenTable.cs` — EF migration
- `Application/UseCase/AuthCase/ForgotPassword.cs` — use case
- `Application/UseCase/AuthCase/ResetPassword.cs` — use case
- `Presentation/Http/Validator/Auth/ForgotPasswordValidator.cs` — input validator
- `Presentation/Http/Validator/Auth/ResetPasswordValidator.cs` — input validator
- `Templates/Email/PasswordResetEmail.html` — email template
- Test files (mirroring structure under `Idiomas.Tests.Core/`)

**Modify:**
- `Application/DTO/Auth.cs` — add `ForgotPasswordDTO`, `ResetPasswordDTO`
- `Infrastructure/Service/DependencyInjection.cs` — register email services
- `Infrastructure/Database/Context/ApplicationContext.cs` — add DbSet + index
- `Infrastructure/Database/DependencyInjection.cs` — register token repository
- `Application/DependencyInjection.cs` — register use cases
- `Interface/Controller/IAuthController.cs` — add method signatures
- `Presentation/Http/Controller/AuthController.cs` — implement endpoints
- `Presentation/Http/Route/AuthRoute.cs` — register routes
- `Presentation/Http/Validator/DependencyInjection.cs` — register validators
- `Idiomas.Core.csproj` — add SendGrid package
- `.env.example` — add new env vars

---

## Task 1: Add SendGrid NuGet Package

**Files:**
- Modify: `Idiomas.Core/Idiomas.Core.csproj`

- [ ] **Step 1: Add SendGrid package**

Run from `IdiomasAPI/Idiomas.Core/`:

```bash
dotnet add package SendGrid
```

- [ ] **Step 2: Verify restore succeeds**

Run: `dotnet restore`
Expected: success, no errors

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Idiomas.Core.csproj
git commit -m "chore: add SendGrid NuGet package"
```

---

## Task 2: IEmailService Interface and EmailMessage Record

**Files:**
- Create: `Idiomas.Core/Interface/Service/IEmailService.cs`

- [ ] **Step 1: Create the interface file**

```csharp
namespace Idiomas.Core.Interface.Service;

public interface IEmailService
{
    public Task SendAsync(EmailMessage message);
}

public record EmailMessage(
    string To,
    string Subject,
    string HtmlBody
);
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Interface/Service/IEmailService.cs
git commit -m "feat: add IEmailService interface and EmailMessage record"
```

---

## Task 3: EmailTemplatePlaceholder Record

**Files:**
- Create: `Idiomas.Core/Infrastructure/Service/Email/EmailTemplatePlaceholder.cs`

- [ ] **Step 1: Create the record**

```csharp
namespace Idiomas.Core.Infrastructure.Service.Email;

public record EmailTemplatePlaceholder(string Key, string Value);
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Infrastructure/Service/Email/EmailTemplatePlaceholder.cs
git commit -m "feat: add EmailTemplatePlaceholder record"
```

---

## Task 4: EmailTemplateLoader

**Files:**
- Create: `Idiomas.Core/Infrastructure/Service/Email/EmailTemplateLoader.cs`
- Test: `Idiomas.Tests.Core/Infrastructure/Service/Email/EmailTemplateLoaderTest.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using Idiomas.Core.Infrastructure.Service.Email;

namespace Idiomas.Tests.Core.Infrastructure.Service.Email;

public class EmailTemplateLoaderTest
{
    [Fact]
    public void Load_ReplacesPlaceholdersInTemplate()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "idiomas_templates_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        string templateContent = "<html><body>Hello {{UserName}}, click <a href=\"{{ResetLink}}\">here</a></body></html>";
        string templatePath = Path.Combine(tempDir, "TestEmail.html");
        File.WriteAllText(templatePath, templateContent);

        var loader = new EmailTemplateLoader(tempDir);

        var placeholders = new List<EmailTemplatePlaceholder>
        {
            new("UserName", "João"),
            new("ResetLink", "https://app.idiomas.com/reset?token=abc123")
        };

        string result = loader.Load("TestEmail.html", placeholders);

        Assert.Contains("Hello João", result);
        Assert.Contains("https://app.idiomas.com/reset?token=abc123", result);
        Assert.DoesNotContain("{{UserName}}", result);
        Assert.DoesNotContain("{{ResetLink}}", result);

        Directory.Delete(tempDir, true);
    }

    [Fact]
    public void Load_ThrowsFileNotFoundExceptionWhenTemplateDoesNotExist()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "idiomas_templates_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        var loader = new EmailTemplateLoader(tempDir);

        Assert.Throws<FileNotFoundException>(() => loader.Load("NonExistent.html", []));

        Directory.Delete(tempDir, true);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test --filter "EmailTemplateLoaderTest"`
Expected: FAIL (class not found)

- [ ] **Step 3: Write minimal implementation**

```csharp
namespace Idiomas.Core.Infrastructure.Service.Email;

public class EmailTemplateLoader(string templatesDirectory)
{
    private readonly string _templatesDirectory = templatesDirectory;

    public string Load(string templateName, IEnumerable<EmailTemplatePlaceholder> placeholders)
    {
        string filePath = Path.Combine(this._templatesDirectory, templateName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Email template '{templateName}' not found at '{filePath}'.", filePath);
        }

        string content = File.ReadAllText(filePath);

        foreach (EmailTemplatePlaceholder placeholder in placeholders)
        {
            content = content.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }

        return content;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test --filter "EmailTemplateLoaderTest"`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Infrastructure/Service/Email/EmailTemplateLoader.cs Idiomas.Tests.Core/Infrastructure/Service/Email/EmailTemplateLoaderTest.cs
git commit -m "feat: add EmailTemplateLoader with placeholder substitution"
```

---

## Task 5: SendGridEmailService

**Files:**
- Create: `Idiomas.Core/Infrastructure/Service/Email/SendGridEmailService.cs`
- Test: `Idiomas.Tests.Core/Infrastructure/Service/Email/SendGridEmailServiceTest.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using Idiomas.Core.Application.Error;
using Idiomas.Core.Infrastructure.Service.Email;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Idiomas.Tests.Core.Infrastructure.Service.Email;

public class SendGridEmailServiceTest
{
    private readonly Mock<ISendGridClient> _sendGridClientMock = new();

    private IConfiguration BuildConfiguration(string apiKey = "SG.test", string senderAddress = "noreply@idiomas.app", string senderName = "Idiomas")
    {
        var configValues = new Dictionary<string, string?>
        {
            { "SendGrid:ApiKey", apiKey },
            { "Email:SenderAddress", senderAddress },
            { "Email:SenderName", senderName }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    [Fact]
    public void Constructor_ThrowsWhenApiKeyIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(apiKey: null!);

        Assert.Throws<InvalidOperationException>(() => new SendGridEmailService(this._sendGridClientMock.Object, config));
    }

    [Fact]
    public void Constructor_ThrowsWhenSenderAddressIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(senderAddress: null!);

        Assert.Throws<InvalidOperationException>(() => new SendGridEmailService(this._sendGridClientMock.Object, config));
    }

    [Fact]
    public async Task SendAsync_ThrowsApiExceptionWhenSendGridFails()
    {
        IConfiguration config = this.BuildConfiguration();

        var failedResponse = new Mock<ISendGridClientResponse>();
        failedResponse.SetupGet(r => r.IsSuccessStatusCode).Returns(false);
        failedResponse.SetupGet(r => r.StatusCode).Returns(System.Net.HttpStatusCode.Unauthorized);
        failedResponse.SetupGet(r => r.Body).Returns(new StringContent("error"));

        this._sendGridClientMock
            .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResponse.Object);

        var service = new SendGridEmailService(this._sendGridClientMock.Object, config);

        var message = new EmailMessage("user@example.com", "Subject", "<p>Body</p>");

        await Assert.ThrowsAsync<ApiException>(() => service.SendAsync(message));
    }

    [Fact]
    public async Task SendAsync_SendsEmailSuccessfully()
    {
        IConfiguration config = this.BuildConfiguration();

        var successResponse = new Mock<ISendGridClientResponse>();
        successResponse.SetupGet(r => r.IsSuccessStatusCode).Returns(true);
        successResponse.SetupGet(r => r.StatusCode).Returns(System.Net.HttpStatusCode.OK);
        successResponse.SetupGet(r => r.Body).Returns(new StringContent(""));

        this._sendGridClientMock
            .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResponse.Object)
            .Callback<SendGridMessage, CancellationToken>((msg, _) =>
            {
                Assert.Equal("user@example.com", msg.Personalizations[0].Tos[0].Email);
                Assert.Equal("Subject", msg.Subject);
            });

        var service = new SendGridEmailService(this._sendGridClientMock.Object, config);

        var message = new EmailMessage("user@example.com", "Subject", "<p>Body</p>");

        await service.SendAsync(message);

        this._sendGridClientMock.Verify(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

- [ ] **Step 2: Create ISendGridClient interface for testability**

The `SendGrid` package's `SendGridClient` is a sealed class. To make it mockable (per the project rule "Sempre que uma lib externa for utilizada, ela deve ser abstraída em um adapter"), we create a thin interface.

Create `Idiomas.Core/Infrastructure/Service/Email/ISendGridClient.cs`:

```csharp
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Idiomas.Core.Infrastructure.Service.Email;

public interface ISendGridClient
{
    Task<ISendGridClientResponse> SendEmailAsync(SendGridMessage msg, CancellationToken cancellationToken = default);
}

public interface ISendGridClientResponse
{
    bool IsSuccessStatusCode { get; }
    System.Net.HttpStatusCode StatusCode { get; }
    HttpContent Body { get; }
}
```

Create `Idiomas.Core/Infrastructure/Service/Email/SendGridClientAdapter.cs`:

```csharp
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Idiomas.Core.Infrastructure.Service.Email;

public class SendGridClientAdapter(SendGridClient sendGridClient) : ISendGridClient
{
    private readonly SendGridClient _sendGridClient = sendGridClient;

    public async Task<ISendGridClientResponse> SendEmailAsync(SendGridMessage msg, CancellationToken cancellationToken = default)
    {
        Response response = await this._sendGridClient.SendEmailAsync(msg, cancellationToken);

        return new SendGridClientResponseAdapter(response);
    }
}

public class SendGridClientResponseAdapter(Response response) : ISendGridClientResponse
{
    private readonly Response _response = response;

    public bool IsSuccessStatusCode => this._response.IsSuccessStatusCode;

    public System.Net.HttpStatusCode StatusCode => this._response.StatusCode;

    public HttpContent Body => this._response.Body;
}
```

- [ ] **Step 3: Run test to verify it fails**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test --filter "SendGridEmailServiceTest"`
Expected: FAIL (SendGridEmailService not found)

- [ ] **Step 4: Write minimal implementation**

```csharp
using System.Net;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Idiomas.Core.Infrastructure.Service.Email;

public class SendGridEmailService(ISendGridClient sendGridClient, IConfiguration configuration) : IEmailService
{
    private readonly ISendGridClient _sendGridClient = sendGridClient;
    private readonly string _apiKey = configuration["SendGrid:ApiKey"] ?? throw new InvalidOperationException("SendGrid:ApiKey is required");
    private readonly string _senderAddress = configuration["Email:SenderAddress"] ?? throw new InvalidOperationException("Email:SenderAddress is required");
    private readonly string _senderName = configuration["Email:SenderName"] ?? throw new InvalidOperationException("Email:SenderName is required");

    public async Task SendAsync(EmailMessage message)
    {
        SendGridMessage emailMessage = MailHelper.CreateSingleEmail(
            new EmailAddress(this._senderAddress, this._senderName),
            new EmailAddress(message.To),
            message.Subject,
            plainTextContent: "",
            htmlContent: message.HtmlBody
        );

        ISendGridClientResponse response = await this._sendGridClient.SendEmailAsync(emailMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException($"Failed to send email to '{message.To}'. Status: {response.StatusCode}", HttpStatusCode.ServiceUnavailable);
        }
    }
}
```

- [ ] **Step 5: Run test to verify it passes**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test --filter "SendGridEmailServiceTest"`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Infrastructure/Service/Email/ISendGridClient.cs Idiomas.Core/Infrastructure/Service/Email/SendGridClientAdapter.cs Idiomas.Core/Infrastructure/Service/Email/SendGridEmailService.cs Idiomas.Tests.Core/Infrastructure/Service/Email/SendGridEmailServiceTest.cs
git commit -m "feat: add SendGridEmailService with ISendGridClient adapter"
```

---

## Task 6: PasswordResetToken Entity

**Files:**
- Create: `Idiomas.Core/Domain/Entity/PasswordResetToken.cs`

- [ ] **Step 1: Create the entity**

```csharp
namespace Idiomas.Core.Domain.Entity;

public class PasswordResetToken(Guid id, Guid userId, string token, DateTime createdAt, DateTime expiresAt, DateTime? usedAt = null)
{
    public Guid Id { get; private set; } = id;
    public Guid UserId { get; private set; } = userId;
    public string Token { get; private set; } = token;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime ExpiresAt { get; private set; } = expiresAt;
    public DateTime? UsedAt { get; set; } = usedAt;

    public bool IsExpired => DateTime.UtcNow > this.ExpiresAt;

    public bool IsUsed => this.UsedAt != null;
}
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Domain/Entity/PasswordResetToken.cs
git commit -m "feat: add PasswordResetToken domain entity"
```

---

## Task 7: PasswordResetTokenModel and Mapping

**Files:**
- Create: `Idiomas.Core/Infrastructure/Database/Model/PasswordResetTokenModel.cs`
- Create: `Idiomas.Core/Infrastructure/Database/Mapper/PasswordResetTokenMappingExtension.cs`

- [ ] **Step 1: Create the EF model**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idiomas.Core.Infrastructure.Database.Model;

[Table("password_reset_token")]
public class PasswordResetTokenModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    [Required]
    [MaxLength(128)]
    [Column("token")]
    public required string Token { get; set; }

    [Required]
    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }

    [Required]
    [Column("expires_at")]
    public required DateTime ExpiresAt { get; set; }

    [Column("used_at")]
    public DateTime? UsedAt { get; set; }

    [ForeignKey("UserId")]
    public UserModel? User { get; set; }
}
```

- [ ] **Step 2: Create the mapping extension**

```csharp
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class PasswordResetTokenMappingExtension
{
    public static PasswordResetToken ToEntity(this PasswordResetTokenModel model)
    {
        return new PasswordResetToken(model.Id, model.UserId, model.Token, model.CreatedAt, model.ExpiresAt, model.UsedAt);
    }

    public static PasswordResetTokenModel ToModel(this PasswordResetToken entity)
    {
        return new PasswordResetTokenModel()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Token = entity.Token,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            UsedAt = entity.UsedAt
        };
    }
}
```

- [ ] **Step 3: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 4: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Infrastructure/Database/Model/PasswordResetTokenModel.cs Idiomas.Core/Infrastructure/Database/Mapper/PasswordResetTokenMappingExtension.cs
git commit -m "feat: add PasswordResetTokenModel and mapping extension"
```

---

## Task 8: IPasswordResetTokenRepository Interface

**Files:**
- Create: `Idiomas.Core/Interface/Repository/IPasswordResetTokenRepository.cs`

- [ ] **Step 1: Create the interface**

```csharp
using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Interface.Repository;

public interface IPasswordResetTokenRepository
{
    public Task Insert(PasswordResetToken token);

    public Task<PasswordResetToken?> GetByToken(string token);

    public Task<PasswordResetToken?> GetActiveTokenByUserId(Guid userId);

    public Task MarkAsUsed(PasswordResetToken token);
}
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Interface/Repository/IPasswordResetTokenRepository.cs
git commit -m "feat: add IPasswordResetTokenRepository interface"
```

---

## Task 9: PasswordResetTokenRepository Implementation

**Files:**
- Create: `Idiomas.Core/Infrastructure/Database/Repository/PasswordResetTokenRepository.cs`

- [ ] **Step 1: Create the repository**

```csharp
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class PasswordResetTokenRepository(ApplicationContext database) : IPasswordResetTokenRepository
{
    private readonly ApplicationContext _database = database;

    public async Task Insert(PasswordResetToken token)
    {
        PasswordResetTokenModel model = token.ToModel();

        this._database.PasswordResetToken.Add(model);

        await this._database.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetByToken(string token)
    {
        PasswordResetTokenModel? model = await this._database.PasswordResetToken
            .FirstOrDefaultAsync(record => record.Token == token);

        return model?.ToEntity();
    }

    public async Task<PasswordResetToken?> GetActiveTokenByUserId(Guid userId)
    {
        PasswordResetTokenModel? model = await this._database.PasswordResetToken
            .Where(record => record.UserId == userId && record.UsedAt == null && record.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(record => record.CreatedAt)
            .FirstOrDefaultAsync();

        return model?.ToEntity();
    }

    public async Task MarkAsUsed(PasswordResetToken token)
    {
        PasswordResetTokenModel? model = await this._database.PasswordResetToken
            .FirstOrDefaultAsync(record => record.Id == token.Id);

        if (model is null)
        {
            throw new KeyNotFoundException($"Password reset token with ID {token.Id} not found.");
        }

        model.UsedAt = DateTime.UtcNow;

        await this._database.SaveChangesAsync();
    }
}
```

- [ ] **Step 2: Verify build (will fail — DbSet not yet added)**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: FAIL — `ApplicationContext` does not have `PasswordResetToken` DbSet

- [ ] **Step 3: Commit (will be fixed in next task together with ApplicationContext)**

Skip commit — this task is completed together with Task 10.

---

## Task 10: Add DbSet to ApplicationContext

**Files:**
- Modify: `Idiomas.Core/Infrastructure/Database/Context/ApplicationContext.cs`

- [ ] **Step 1: Add DbSet and index**

Add after the `ScenarioModel` DbSet (line 15):

```csharp
public DbSet<PasswordResetTokenModel> PasswordResetToken { get; set; }
```

Add inside `OnModelCreating`, after the scenario index:

```csharp
modelBuilder.Entity<PasswordResetTokenModel>()
    .HasIndex(token => token.Token)
    .IsUnique();

modelBuilder.Entity<PasswordResetTokenModel>()
    .HasIndex(token => token.UserId);
```

Full updated `OnModelCreating` section:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<ConversationModel>()
        .HasIndex(c => c.UserId);

    modelBuilder.Entity<ConversationModel>()
        .HasIndex(c => c.IsActive);

    modelBuilder.Entity<MessageModel>()
        .HasIndex(m => m.ConversationId);

    modelBuilder.Entity<MessageModel>()
        .HasIndex(m => new { m.ConversationId, m.CreatedAt });

    modelBuilder.Entity<CorrectionModel>()
        .HasIndex(c => c.MessageId);

    modelBuilder.Entity<ScenarioModel>()
        .HasIndex(s => s.Language);

    modelBuilder.Entity<PasswordResetTokenModel>()
        .HasIndex(token => token.Token)
        .IsUnique();

    modelBuilder.Entity<PasswordResetTokenModel>()
        .HasIndex(token => token.UserId);
}
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit (Tasks 9 + 10 together)**

```bash
cd IdiomasAPI
git add Idiomas.Core/Infrastructure/Database/Repository/PasswordResetTokenRepository.cs Idiomas.Core/Infrastructure/Database/Context/ApplicationContext.cs
git commit -m "feat: add PasswordResetTokenRepository and DbSet registration"
```

---

## Task 11: Register Repositories in DI

**Files:**
- Modify: `Idiomas.Core/Infrastructure/Database/DependencyInjection.cs`

- [ ] **Step 1: Add token repository registration**

Add after `services.AddScoped<IScenarioRepository, ScenarioRepository>();`:

```csharp
services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
```

Add `using Idiomas.Core.Interface.Repository;` if not already present (it is).

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Infrastructure/Database/DependencyInjection.cs
git commit -m "feat: register IPasswordResetTokenRepository in DI"
```

---

## Task 12: Create EF Migration

**Files:**
- Create: `Idiomas.Core/Infrastructure/Database/Migrations/<generated>_CreatePasswordResetTokenTable.cs`

- [ ] **Step 1: Generate migration**

Run from `IdiomasAPI/Idiomas.Core/`:

```bash
dotnet ef migrations add CreatePasswordResetTokenTable
```

Expected: migration files created in `Infrastructure/Database/Migrations/`

- [ ] **Step 2: Verify the migration includes the table**

Open the generated migration file and confirm it creates a `password_reset_token` table with columns: `id`, `user_id`, `token`, `expires_at`, `used_at`, and a unique index on `token`.

- [ ] **Step 3: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 4: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Infrastructure/Database/Migrations/
git commit -m "feat: add CreatePasswordResetTokenTable migration"
```

---

## Task 13: Application DTOs

**Files:**
- Modify: `Idiomas.Core/Application/DTO/Auth.cs`

- [ ] **Step 1: Add new DTOs**

Replace the entire file content:

```csharp
namespace Idiomas.Core.Application.DTO.Auth;

public record MailPasswordLoginDTO(string Email, string Password);

public record ForgotPasswordDTO(string Email);

public record ResetPasswordDTO(string Token, string NewPassword);
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Application/DTO/Auth.cs
git commit -m "feat: add ForgotPasswordDTO and ResetPasswordDTO"
```

---

## Task 14: Password Reset Email Template

**Files:**
- Create: `Idiomas.Core/Templates/Email/PasswordResetEmail.html`

- [ ] **Step 1: Create the template**

```html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Redefinição de Senha</title>
</head>
<body style="font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;">
    <h1 style="color: #2563eb;">Redefinição de Senha</h1>

    <p>Olá, {{UserName}}!</p>

    <p>Recebemos uma solicitação para redefinir sua senha. Clique no link abaixo para criar uma nova senha:</p>

    <p>
        <a href="{{ResetLink}}" style="display: inline-block; background-color: #2563eb; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold;">
            Redefinir Senha
        </a>
    </p>

    <p style="color: #6b7280; font-size: 14px;">
        Este link expira em 1 hora. Se você não solicitou esta redefinição, ignore este email.
    </p>

    <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 24px 0;">

    <p style="color: #9ca3af; font-size: 12px;">
        Idiomas — Aprenda idiomas conversando com IA
    </p>
</body>
</html>
```

- [ ] **Step 2: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Templates/Email/PasswordResetEmail.html
git commit -m "feat: add PasswordResetEmail HTML template"
```

---

## Task 15: Register Email Services in DI

**Files:**
- Modify: `Idiomas.Core/Infrastructure/Service/DependencyInjection.cs`

- [ ] **Step 1: Add email service registrations**

Add `using Idiomas.Core.Infrastructure.Service.Email;` to the top.

Add after the LLM service registration (before `return services;`):

```csharp
// Email Service
string sendGridApiKey = configuration["SendGrid:ApiKey"] ?? throw new InvalidOperationException("SendGrid:ApiKey is required");
string templatesDirectory = Path.Combine(AppContext.BaseDirectory, "Templates", "Email");

services.AddScoped<EmailTemplateLoader>(provider =>
{
    return new EmailTemplateLoader(templatesDirectory);
});

services.AddScoped<ISendGridClient>(provider =>
{
    var sendGridClient = new SendGridClient(sendGridApiKey);
    return new SendGridClientAdapter(sendGridClient);
});

services.AddScoped<IEmailService, SendGridEmailService>();
```

Add `using SendGrid;` to the top.

- [ ] **Step 2: Configure templates to be copied to output directory**

Edit `Idiomas.Core/Idiomas.Core.csproj` and add inside `<Project>` (or `<ItemGroup>`):

```xml
<ItemGroup>
  <None Include="Templates\Email\**\*" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

- [ ] **Step 3: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 4: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Infrastructure/Service/DependencyInjection.cs Idiomas.Core/Idiomas.Core.csproj
git commit -m "feat: register email services and template loader in DI"
```

---

## Task 16: ForgotPassword Use Case

**Files:**
- Create: `Idiomas.Core/Application/UseCase/AuthCase/ForgotPassword.cs`
- Test: `Idiomas.Tests.Core/Application/UseCase/AuthCase/ForgotPasswordTest.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.AuthCase;

public class ForgotPasswordTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordResetTokenRepository> _tokenRepositoryMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<EmailTemplateLoader> _templateLoaderMock;
    private readonly Mock<IConfiguration> _configurationMock = new();

    public ForgotPasswordTest()
    {
        this._templateLoaderMock = new Mock<EmailTemplateLoader>(Path.Combine(Path.GetTempPath(), "fake"));
        this._configurationMock.SetupGet(config => config["FrontendUrl"]).Returns("https://app.idiomas.com");
    }

    [Fact]
    public async Task Execute_ReturnsSilentlyWhenEmailDoesNotExist()
    {
        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var useCase = new ForgotPassword(
            this._userRepositoryMock.Object,
            this._tokenRepositoryMock.Object,
            this._emailServiceMock.Object,
            this._templateLoaderMock.Object,
            this._configurationMock.Object
        );

        var dto = new ForgotPasswordDTO("nonexistent@example.com");

        await useCase.Execute(dto);

        this._tokenRepositoryMock.Verify(repository => repository.GetActiveTokenByUserId(It.IsAny<Guid>()), Times.Never);
        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<PasswordResetToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenActiveTokenAlreadyExists()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", "hashed");
        var activeToken = new PasswordResetToken(Guid.NewGuid(), Guid.Parse(user.Id), "existing-token", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._tokenRepositoryMock
            .Setup(repository => repository.GetActiveTokenByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(activeToken);

        var useCase = new ForgotPassword(
            this._userRepositoryMock.Object,
            this._tokenRepositoryMock.Object,
            this._emailServiceMock.Object,
            this._templateLoaderMock.Object,
            this._configurationMock.Object
        );

        var dto = new ForgotPasswordDTO("joao@example.com");

        var exception = await Assert.ThrowsAsync<ApiException>(() => useCase.Execute(dto));

        Assert.Equal(System.Net.HttpStatusCode.Conflict, exception.StatusCode);
        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<PasswordResetToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_GeneratesTokenAndSendsEmailWhenNoActiveTokenExists()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", "hashed");

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._tokenRepositoryMock
            .Setup(repository => repository.GetActiveTokenByUserId(It.IsAny<Guid>()))
            .ReturnsAsync((PasswordResetToken?)null);

        this._templateLoaderMock
            .Setup(loader => loader.Load(It.IsAny<string>(), It.IsAny<IEnumerable<EmailTemplatePlaceholder>>()))
            .Returns("<html>email</html>");

        var useCase = new ForgotPassword(
            this._userRepositoryMock.Object,
            this._tokenRepositoryMock.Object,
            this._emailServiceMock.Object,
            this._templateLoaderMock.Object,
            this._configurationMock.Object
        );

        var dto = new ForgotPasswordDTO("joao@example.com");

        await useCase.Execute(dto);

        this._tokenRepositoryMock.Verify(repository => repository.GetActiveTokenByUserId(Guid.Parse(user.Id)), Times.Once);
        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.Is<PasswordResetToken>(token =>
            token.UserId == Guid.Parse(user.Id) &&
            !string.IsNullOrEmpty(token.Token) &&
            token.ExpiresAt > DateTime.UtcNow
        )), Times.Once);
        this._emailServiceMock.Verify(service => service.SendAsync(It.Is<EmailMessage>(message =>
            message.To == "joao@example.com"
        )), Times.Once);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test --filter "ForgotPasswordTest"`
Expected: FAIL (class not found)

- [ ] **Step 3: Write minimal implementation**

```csharp
using System.Security.Cryptography;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class ForgotPassword(
    IUserRepository userRepository,
    IPasswordResetTokenRepository tokenRepository,
    IEmailService emailService,
    EmailTemplateLoader templateLoader,
    IConfiguration configuration)
{
    private const int TOKEN_LENGTH = 64;

    private const int TOKEN_EXPIRATION_HOURS = 1;

    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository = tokenRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailTemplateLoader _templateLoader = templateLoader;
    private readonly IConfiguration _configuration = configuration;

    public async Task Execute(ForgotPasswordDTO dto)
    {
        User? user = await this._userRepository.GetByEmail(dto.Email);

        if (user == null)
        {
            return;
        }

        Guid userId = Guid.Parse(user.Id);

        await this.EnsureNoActiveTokenExists(userId);

        PasswordResetToken token = this.CreatePasswordResetToken(userId);

        await this._tokenRepository.Insert(token);

        await this.SendPasswordResetEmail(user, token.Token);
    }

    private async Task EnsureNoActiveTokenExists(Guid userId)
    {
        PasswordResetToken? activeToken = await this._tokenRepository.GetActiveTokenByUserId(userId);

        if (activeToken != null)
        {
            throw new ApiException("Já existe uma solicitação de redefinição de senha ativa. Verifique seu email ou aguarde a expiração.", HttpStatusCode.Conflict);
        }
    }

    private PasswordResetToken CreatePasswordResetToken(Guid userId)
    {
        string tokenValue = GenerateSecureToken();
        DateTime expiresAt = DateTime.UtcNow.AddHours(TOKEN_EXPIRATION_HOURS);

        return new PasswordResetToken(Guid.NewGuid(), userId, tokenValue, DateTime.UtcNow, expiresAt);
    }

    private async Task SendPasswordResetEmail(User user, string tokenValue)
    {
        string frontendUrl = this._configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl is not configured");
        string resetLink = $"{frontendUrl}/reset-password?token={tokenValue}";

        string htmlBody = this._templateLoader.Load("PasswordResetEmail.html", [
            new EmailTemplatePlaceholder("UserName", user.Name),
            new EmailTemplatePlaceholder("ResetLink", resetLink)
        ]);

        var emailMessage = new EmailMessage(user.Email, "Redefinição de senha", htmlBody);

        await this._emailService.SendAsync(emailMessage);
    }

    private static string GenerateSecureToken()
    {
        return RandomNumberGenerator.GetHexString(TOKEN_LENGTH);
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test --filter "ForgotPasswordTest"`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Application/UseCase/AuthCase/ForgotPassword.cs Idiomas.Tests.Core/Application/UseCase/AuthCase/ForgotPasswordTest.cs
git commit -m "feat: add ForgotPassword use case with token generation and email sending"
```

---

## Task 17: ResetPassword Use Case

**Files:**
- Create: `Idiomas.Core/Application/UseCase/AuthCase/ResetPassword.cs`
- Test: `Idiomas.Tests.Core/Application/UseCase/AuthCase/ResetPasswordTest.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Moq;
using System.Net;

namespace Idiomas.Tests.Core.Application.UseCase.AuthCase;

public class ResetPasswordTest
{
    private readonly Mock<IPasswordResetTokenRepository> _tokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IHash> _hashMock = new();

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenTokenDoesNotExist()
    {
        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync((PasswordResetToken?)null);

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("invalid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<ApiException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenTokenIsExpired()
    {
        var token = new PasswordResetToken(Guid.NewGuid(), Guid.NewGuid(), "valid-token", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync(token);

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<ApiException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenTokenIsAlreadyUsed()
    {
        var token = new PasswordResetToken(Guid.NewGuid(), Guid.NewGuid(), "valid-token", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddMinutes(-5));

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync(token);

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<ApiException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_UpdatesPasswordAndMarksTokenAsUsedWhenValid()
    {
        Guid userId = Guid.NewGuid();
        var token = new PasswordResetToken(Guid.NewGuid(), userId, "valid-token", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new User(userId.ToString(), "João", "joao@example.com", "oldhash");

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync(token);

        this._userRepositoryMock
            .Setup(repository => repository.GetById(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._hashMock
            .Setup(hash => hash.Hash(It.IsAny<string>()))
            .Returns("newhash");

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        await useCase.Execute(dto);

        Assert.Equal("newhash", user.Password);

        this._userRepositoryMock.Verify(repository => repository.Update(It.Is<User>(updatedUser => updatedUser.Password == "newhash")), Times.Once);
        this._tokenRepositoryMock.Verify(repository => repository.MarkAsUsed(token), Times.Once);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test --filter "ResetPasswordTest"`
Expected: FAIL (class not found)

- [ ] **Step 3: Write minimal implementation**

```csharp
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class ResetPassword(
    IPasswordResetTokenRepository tokenRepository,
    IUserRepository userRepository,
    IHash hash)
{
    private readonly IPasswordResetTokenRepository _tokenRepository = tokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHash _hash = hash;

    public async Task Execute(ResetPasswordDTO dto)
    {
        PasswordResetToken? token = await this._tokenRepository.GetByToken(dto.Token);

        if (token == null || token.IsExpired || token.IsUsed)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        User? user = await this._userRepository.GetById(token.UserId.ToString());

        if (user == null)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        user.Password = this._hash.Hash(dto.NewPassword);

        await this._userRepository.Update(user);

        await this._tokenRepository.MarkAsUsed(token);
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test --filter "ResetPasswordTest"`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Application/UseCase/AuthCase/ResetPassword.cs Idiomas.Tests.Core/Application/UseCase/AuthCase/ResetPasswordTest.cs
git commit -m "feat: add ResetPassword use case with token validation"
```

---

## Task 18: Register Use Cases in DI

**Files:**
- Modify: `Idiomas.Core/Application/DependencyInjection.cs`

- [ ] **Step 1: Add use case registrations**

Add after `services.AddScoped<MailPasswordLogin>();`:

```csharp
services.AddScoped<ForgotPassword>();
services.AddScoped<ResetPassword>();
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Application/DependencyInjection.cs
git commit -m "feat: register ForgotPassword and ResetPassword use cases in DI"
```

---

## Task 19: ForgotPasswordValidator

**Files:**
- Create: `Idiomas.Core/Presentation/Http/Validator/Auth/ForgotPasswordValidator.cs`

- [ ] **Step 1: Create the validator**

```csharp
using System.Net;
using System.Text.RegularExpressions;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public partial class ForgotPasswordValidator : IValidator<ForgotPasswordDTO>
{
    public void Validate(ForgotPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ApiException("Email é obrigatório", HttpStatusCode.BadRequest);
        }

        if (!EmailRegex().IsMatch(dto.Email))
        {
            throw new ApiException("Email inválido", HttpStatusCode.BadRequest);
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Presentation/Http/Validator/Auth/ForgotPasswordValidator.cs
git commit -m "feat: add ForgotPasswordValidator"
```

---

## Task 20: ResetPasswordValidator

**Files:**
- Create: `Idiomas.Core/Presentation/Http/Validator/Auth/ResetPasswordValidator.cs`

- [ ] **Step 1: Create the validator**

```csharp
using System.Net;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public class ResetPasswordValidator : IValidator<ResetPasswordDTO>
{
    private const int MIN_PASSWORD_LENGTH = 8;

    public void Validate(ResetPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token))
        {
            throw new ApiException("Token é obrigatório", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            throw new ApiException("Nova senha é obrigatória", HttpStatusCode.BadRequest);
        }

        if (dto.NewPassword.Length < MIN_PASSWORD_LENGTH)
        {
            throw new ApiException($"A senha deve ter no mínimo {MIN_PASSWORD_LENGTH} caracteres", HttpStatusCode.BadRequest);
        }
    }
}
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Presentation/Http/Validator/Auth/ResetPasswordValidator.cs
git commit -m "feat: add ResetPasswordValidator"
```

---

## Task 21: Register Validators in DI

**Files:**
- Modify: `Idiomas.Core/Presentation/Http/Validator/DependencyInjection.cs`

- [ ] **Step 1: Add validator registrations**

Add after `services.AddScoped<MailPasswordLoginValidator>();`:

```csharp
services.AddScoped<ForgotPasswordValidator>();
services.AddScoped<ResetPasswordValidator>();
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Presentation/Http/Validator/DependencyInjection.cs
git commit -m "feat: register ForgotPassword and ResetPassword validators in DI"
```

---

## Task 22: Update IAuthController Interface

**Files:**
- Modify: `Idiomas.Core/Interface/Controller/IAuthController.cs`

- [ ] **Step 1: Add method signatures**

Replace the entire file:

```csharp
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;

namespace Idiomas.Core.Interface.Controller;

public interface IAuthController
{
    public Task<IResult> MailPasswordLogin(HttpContext httpContext, MailPasswordLoginDTO dto, MailPasswordLogin useCase);

    public Task<IResult> ForgotPassword(ForgotPasswordDTO dto, ForgotPassword useCase);

    public Task<IResult> ResetPassword(ResetPasswordDTO dto, ResetPassword useCase);
}
```

- [ ] **Step 2: Verify build (will fail — AuthController not yet updated)**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: FAIL — AuthController does not implement new methods

Skip commit — completed together with Task 23.

---

## Task 23: Update AuthController Implementation

**Files:**
- Modify: `Idiomas.Core/Presentation/Http/Controller/AuthController.cs`

- [ ] **Step 1: Add endpoint implementations**

Replace the entire file:

```csharp
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Service;
using Idiomas.Core.Presentation.DTO.Auth;
using Idiomas.Core.Presentation.Mapper;

namespace Idiomas.Core.Presentation.Http.Controller;

public class AuthController(IToken tokenGenerator) : IAuthController
{
    private readonly IToken _tokenGenerator = tokenGenerator;

    public async Task<IResult> MailPasswordLogin(HttpContext httpContext, MailPasswordLoginDTO dto, MailPasswordLogin useCase)
    {
        User user = await useCase.Execute(dto);

        MailPasswordLoginResponseDTO response = new()
        {
            User = user.ToResponseDTO(),
            Token = this._tokenGenerator.Generate(user)
        };

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        };

        httpContext.Response.Cookies.Append("Authorization", response.Token, cookieOptions);

        return TypedResults.Ok(response);
    }

    public async Task<IResult> ForgotPassword(ForgotPasswordDTO dto, ForgotPassword useCase)
    {
        await useCase.Execute(dto);

        return TypedResults.Ok();
    }

    public async Task<IResult> ResetPassword(ResetPasswordDTO dto, ResetPassword useCase)
    {
        await useCase.Execute(dto);

        return TypedResults.Ok();
    }
}
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit (Tasks 22 + 23 together)**

```bash
cd IdiomasAPI
git add Idiomas.Core/Interface/Controller/IAuthController.cs Idiomas.Core/Presentation/Http/Controller/AuthController.cs
git commit -m "feat: add ForgotPassword and ResetPassword endpoints to AuthController"
```

---

## Task 24: Register Routes

**Files:**
- Modify: `Idiomas.Core/Presentation/Http/Route/AuthRoute.cs`

- [ ] **Step 1: Add route registrations**

Replace the entire file:

```csharp
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Route;
using Idiomas.Core.Presentation.DTO.Auth;
using Idiomas.Core.Presentation.Http.Validator;
using Idiomas.Core.Presentation.Http.Validator.Auth;

namespace Idiomas.Core.Presentation.Http.Route;

public class AuthRoute(IAuthController controller) : IRoute
{
    private readonly IAuthController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/auth/login", this._controller.MailPasswordLogin)
            .Produces<MailPasswordLoginResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithValidation<MailPasswordLoginValidator, MailPasswordLoginDTO>();

        app.MapPost("/auth/forgot-password", this._controller.ForgotPassword)
            .Produces(StatusCodes.Status200OK)
            .WithValidation<ForgotPasswordValidator, ForgotPasswordDTO>();

        app.MapPost("/auth/reset-password", this._controller.ResetPassword)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithValidation<ResetPasswordValidator, ResetPasswordDTO>();
    }
}
```

- [ ] **Step 2: Verify build**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet build`
Expected: success

- [ ] **Step 3: Commit**

```bash
cd IdiomasAPI
git add Idiomas.Core/Presentation/Http/Route/AuthRoute.cs
git commit -m "feat: add forgot-password and reset-password routes"
```

---

## Task 25: Update .env.example

**Files:**
- Modify: `IdiomasAPI/.env.example`

- [ ] **Step 1: Add new env vars**

Add at the end of the file:

```
SendGrid__ApiKey=your_sendgrid_api_key_here
Email__SenderAddress=noreply@idiomas.app
Email__SenderName=Idiomas
FrontendUrl=https://app.idiomas.com
```

- [ ] **Step 2: Commit**

```bash
cd IdiomasAPI
git add .env.example
git commit -m "docs: add SendGrid and FrontendUrl env vars to .env.example"
```

---

## Task 26: Run Full Test Suite and Static Analysis

- [ ] **Step 1: Run all tests**

Run from `IdiomasAPI/Idiomas.Tests.Core/`: `dotnet test`
Expected: all tests PASS

- [ ] **Step 2: Run static analysis**

Run from `IdiomasAPI/Idiomas.Core/`: `dotnet format --verify-no-changes` or `dotnet build --no-incremental`
Expected: no warnings or errors

- [ ] **Step 3: If any issues, fix and commit**

```bash
cd IdiomasAPI
git add -A
git commit -m "fix: resolve test and analysis issues"
```

If no issues, skip this step.
