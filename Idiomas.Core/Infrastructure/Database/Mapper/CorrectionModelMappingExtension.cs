using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class CorrectionModelMappingExtension
{
    public static Correction ToEntity(this CorrectionModel model, IEncryptionService encryptionService)
    {
        if (!Enum.TryParse<ErrorType>(model.Type, out var errorType))
        {
            throw new InvalidOperationException($"Invalid error type value in database: {model.Type}");
        }

        string decryptedOriginalFragment = encryptionService.Decrypt(model.OriginalFragment);
        string decryptedSuggestedFragment = encryptionService.Decrypt(model.SuggestedFragment);
        string decryptedExplanation = encryptionService.Decrypt(model.Explanation);

        Correction correction = new(
            model.Id.ToString(),
            model.MessageId.ToString(),
            decryptedOriginalFragment,
            decryptedSuggestedFragment,
            decryptedExplanation,
            errorType
        );

        return correction;
    }

    public static CorrectionModel ToModel(this Correction entity, IEncryptionService encryptionService)
    {
        string encryptedOriginalFragment = encryptionService.Encrypt(entity.OriginalFragment);
        string encryptedSuggestedFragment = encryptionService.Encrypt(entity.SuggestedFragment);
        string encryptedExplanation = encryptionService.Encrypt(entity.Explanation);

        CorrectionModel model = new()
        {
            Id = Guid.Parse(entity.Id),
            MessageId = Guid.Parse(entity.MessageId),
            OriginalFragment = encryptedOriginalFragment,
            SuggestedFragment = encryptedSuggestedFragment,
            Explanation = encryptedExplanation,
            Type = entity.Type.ToString(),
            CreatedAt = entity.CreatedAt
        };

        return model;
    }

    public static IEnumerable<Correction> ToEntities(this IEnumerable<CorrectionModel> models, IEncryptionService encryptionService)
    {
        return models.Select(m => m.ToEntity(encryptionService));
    }
}
