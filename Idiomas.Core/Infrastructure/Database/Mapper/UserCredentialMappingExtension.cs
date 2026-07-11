using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class UserCredentialMappingExtension
{
    public static UserCredential ToEntity(this UserCredentialModel model)
    {
        return new UserCredential(
            model.Id.ToString(),
            model.UserId.ToString(),
            model.Provider,
            model.PasswordHash,
            model.ExternalSubject
        );
    }

    public static UserCredentialModel ToModel(this UserCredential entity)
    {
        return new UserCredentialModel
        {
            Id = Guid.Parse(entity.Id),
            UserId = Guid.Parse(entity.UserId),
            Provider = entity.Provider,
            PasswordHash = entity.PasswordHash,
            ExternalSubject = entity.ExternalSubject,
            CreatedAt = DateTime.UtcNow
        };
    }
}
