using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class EmailVerificationTokenMappingExtension
{
    public static EmailVerificationToken ToEntity(this EmailVerificationTokenModel model)
    {
        return new EmailVerificationToken(
            model.Id,
            model.UserId,
            model.TokenHash,
            model.CreatedAt,
            model.ExpiresAt,
            model.UsedAt
        );
    }

    public static EmailVerificationTokenModel ToModel(this EmailVerificationToken entity)
    {
        return new EmailVerificationTokenModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            TokenHash = entity.TokenHash,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            UsedAt = entity.UsedAt
        };
    }
}
