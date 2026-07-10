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
