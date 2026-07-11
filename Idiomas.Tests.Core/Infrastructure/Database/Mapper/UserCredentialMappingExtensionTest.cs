using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Tests.Core.Infrastructure.Database.Mapper;

public class UserCredentialMappingExtensionTest
{
    [Fact]
    public void ToEntity_MapsModelToEntityCorrectly()
    {
        UserCredentialModel model = new()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Provider = AuthProvider.Local,
            PasswordHash = "hashed-password",
            ExternalSubject = null,
            CreatedAt = DateTime.UtcNow
        };

        UserCredential entity = model.ToEntity();

        Assert.Equal(model.Id.ToString(), entity.Id);
        Assert.Equal(model.UserId.ToString(), entity.UserId);
        Assert.Equal(model.Provider, entity.Provider);
        Assert.Equal(model.PasswordHash, entity.PasswordHash);
        Assert.Equal(model.ExternalSubject, entity.ExternalSubject);
    }

    [Fact]
    public void ToModel_MapsEntityToModelCorrectly()
    {
        UserCredential entity = new(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            AuthProvider.Google,
            null,
            "google-sub-123"
        );

        UserCredentialModel model = entity.ToModel();

        Assert.Equal(Guid.Parse(entity.Id), model.Id);
        Assert.Equal(Guid.Parse(entity.UserId), model.UserId);
        Assert.Equal(entity.Provider, model.Provider);
        Assert.Equal(entity.PasswordHash, model.PasswordHash);
        Assert.Equal(entity.ExternalSubject, model.ExternalSubject);
    }
}
