using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class EmailChangeRequestMappingExtension
{
    public static EmailChangeRequest ToEntity(this EmailChangeRequestModel model)
    {
        return new EmailChangeRequest(
            model.Id,
            model.UserId,
            model.NewEmail,
            model.TokenHash,
            model.CreatedAt,
            model.ExpiresAt,
            model.UsedAt
        );
    }

    public static EmailChangeRequestModel ToModel(this EmailChangeRequest entity)
    {
        return new EmailChangeRequestModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            NewEmail = entity.NewEmail,
            TokenHash = entity.TokenHash,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            UsedAt = entity.UsedAt
        };
    }
}
