using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Infrastructure.Database.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Idiomas.Tests.Core.Infrastructure.Database.Repository;

public class UserCredentialRepositoryTest : IDisposable
{
    private readonly ApplicationContext _database;
    private readonly UserCredentialRepository _repository;

    public UserCredentialRepositoryTest()
    {
        ServiceCollection services = new();
        services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        ServiceProvider provider = services.BuildServiceProvider();
        this._database = provider.GetRequiredService<ApplicationContext>();
        this._repository = new UserCredentialRepository(this._database);

        this._database.Database.EnsureCreated();
    }

    [Fact]
    public async Task Insert_PersistsCredential()
    {
        Guid userId = Guid.NewGuid();
        UserCredential credential = new(
            Guid.NewGuid().ToString(),
            userId.ToString(),
            AuthProvider.Local,
            "hashed-password",
            null
        );

        await this._repository.Insert(credential);

        UserCredentialModel? persisted = await this._database.UserCredential
            .FirstOrDefaultAsync(credentialModel => credentialModel.Id == Guid.Parse(credential.Id));

        Assert.NotNull(persisted);
        Assert.Equal("hashed-password", persisted.PasswordHash);
    }

    [Fact]
    public async Task GetByExternalSubject_ReturnsCredentialWhenExists()
    {
        string subject = "google-sub-123";
        UserCredentialModel model = new()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Provider = AuthProvider.Google,
            PasswordHash = null,
            ExternalSubject = subject,
            CreatedAt = DateTime.UtcNow
        };

        this._database.UserCredential.Add(model);
        await this._database.SaveChangesAsync();

        UserCredential? result = await this._repository.GetByExternalSubject(AuthProvider.Google, subject);

        Assert.NotNull(result);
        Assert.Equal(subject, result.ExternalSubject);
    }

    [Fact]
    public async Task GetByExternalSubject_ReturnsNullWhenNotExists()
    {
        UserCredential? result = await this._repository.GetByExternalSubject(AuthProvider.Google, "nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAndProvider_ReturnsCredentialWhenExists()
    {
        Guid userId = Guid.NewGuid();
        UserCredentialModel model = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Provider = AuthProvider.Local,
            PasswordHash = "hashed",
            ExternalSubject = null,
            CreatedAt = DateTime.UtcNow
        };

        this._database.UserCredential.Add(model);
        await this._database.SaveChangesAsync();

        UserCredential? result = await this._repository.GetByUserIdAndProvider(userId.ToString(), AuthProvider.Local);

        Assert.NotNull(result);
        Assert.Equal("hashed", result.PasswordHash);
    }

    [Fact]
    public async Task GetByUserIdAndProvider_ReturnsNullWhenNotExists()
    {
        UserCredential? result = await this._repository.GetByUserIdAndProvider(Guid.NewGuid().ToString(), AuthProvider.Local);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAndProvider_ReturnsNullForInvalidGuid()
    {
        UserCredential? result = await this._repository.GetByUserIdAndProvider("not-a-guid", AuthProvider.Local);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_UpdatesPasswordHash()
    {
        Guid userId = Guid.NewGuid();
        UserCredentialModel model = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Provider = AuthProvider.Local,
            PasswordHash = "old-hash",
            ExternalSubject = null,
            CreatedAt = DateTime.UtcNow
        };

        this._database.UserCredential.Add(model);
        await this._database.SaveChangesAsync();

        UserCredential credential = model.ToEntity();
        credential.UpdatePasswordHash("new-hash");

        await this._repository.Update(credential);

        UserCredentialModel? updated = await this._database.UserCredential
            .FirstOrDefaultAsync(credentialModel => credentialModel.Id == model.Id);

        Assert.NotNull(updated);
        Assert.Equal("new-hash", updated.PasswordHash);
    }

    public void Dispose()
    {
        this._database.Database.EnsureDeleted();
        this._database.Dispose();
    }
}
