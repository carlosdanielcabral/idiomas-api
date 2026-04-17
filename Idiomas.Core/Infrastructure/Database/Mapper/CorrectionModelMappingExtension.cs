using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class CorrectionModelMappingExtension
{
    public static Correction ToEntity(this CorrectionModel model)
    {
        if (!Enum.TryParse<ErrorType>(model.Type, out var errorType))
        {
            throw new InvalidOperationException($"Invalid error type value in database: {model.Type}");
        }

        Correction correction = new(
            model.Id.ToString(),
            model.MessageId.ToString(),
            model.OriginalFragment,
            model.SuggestedFragment,
            model.Explanation,
            errorType
        );

        return correction;
    }

    public static CorrectionModel ToModel(this Correction entity)
    {
        CorrectionModel model = new()
        {
            Id = Guid.Parse(entity.Id),
            MessageId = Guid.Parse(entity.MessageId),
            OriginalFragment = entity.OriginalFragment,
            SuggestedFragment = entity.SuggestedFragment,
            Explanation = entity.Explanation,
            Type = entity.Type.ToString(),
            CreatedAt = entity.CreatedAt
        };

        return model;
    }

    public static IEnumerable<Correction> ToEntities(this IEnumerable<CorrectionModel> models)
    {
        return models.Select(m => m.ToEntity());
    }
}
