using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class MessageModelMapper
{
    public static Message ToEntity(this MessageModel model)
    {
        if (!Enum.TryParse<MessageRole>(model.Role, out var role))
        {
            throw new InvalidOperationException($"Invalid role value in database: {model.Role}");
        }

        Message message = new(
            model.Id.ToString(),
            model.ConversationId.ToString(),
            role,
            model.Content
        );

        foreach (CorrectionModel correctionModel in model.Corrections)
        {
            message.AddCorrection(correctionModel.ToEntity());
        }

        return message;
    }

    public static MessageModel ToModel(this Message entity)
    {
        MessageModel model = new()
        {
            Id = Guid.Parse(entity.Id),
            ConversationId = Guid.Parse(entity.ConversationId),
            Role = entity.Role.ToString(),
            Content = entity.Content,
            CreatedAt = entity.CreatedAt
        };

        return model;
    }

    public static IEnumerable<Message> ToEntities(this IEnumerable<MessageModel> models)
    {
        return models.Select(m => m.ToEntity());
    }
}
