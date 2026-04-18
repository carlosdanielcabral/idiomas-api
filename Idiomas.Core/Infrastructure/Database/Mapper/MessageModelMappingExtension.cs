using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class MessageModelMappingExtension
{
    public static Message ToEntity(this MessageModel model, IEncryptionService encryptionService)
    {
        if (!Enum.TryParse<MessageRole>(model.Role, out var role))
        {
            throw new InvalidOperationException($"Invalid role value in database: {model.Role}");
        }

        string decryptedContent = encryptionService.Decrypt(model.Content);

        Message message = new(
            model.Id.ToString(),
            model.ConversationId.ToString(),
            role,
            decryptedContent
        );

        foreach (CorrectionModel correctionModel in model.Corrections)
        {
            message.AddCorrection(correctionModel.ToEntity(encryptionService));
        }

        return message;
    }

    public static MessageModel ToModel(this Message entity, IEncryptionService encryptionService)
    {
        string encryptedContent = encryptionService.Encrypt(entity.Content);

        MessageModel model = new()
        {
            Id = Guid.Parse(entity.Id),
            ConversationId = Guid.Parse(entity.ConversationId),
            Role = entity.Role.ToString(),
            Content = encryptedContent,
            CreatedAt = entity.CreatedAt
        };

        return model;
    }

    public static IEnumerable<Message> ToEntities(this IEnumerable<MessageModel> models, IEncryptionService encryptionService)
    {
        return models.Select(m => m.ToEntity(encryptionService));
    }
}
