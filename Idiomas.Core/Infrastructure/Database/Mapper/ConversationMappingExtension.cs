using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class ConversationMappingExtension
{
    public static Conversation ToEntity(this ConversationModel model)
    {
        if (!Enum.TryParse<Language>(model.Language, out var language))
        {
            throw new InvalidOperationException($"Invalid language value in database: {model.Language}");
        }

        if (!Enum.TryParse<ConversationMode>(model.Mode, out var mode))
        {
            throw new InvalidOperationException($"Invalid mode value in database: {model.Mode}");
        }

        string? scenarioId = model.ScenarioId.HasValue ? model.ScenarioId.Value.ToString() : null;

        Conversation conversation = new(
            model.Id.ToString(),
            model.UserId.ToString(),
            language,
            mode,
            scenarioId
        );

        foreach (MessageModel messageModel in model.Messages)
        {
            conversation.AddMessage(messageModel.ToEntity());
        }

        return conversation;
    }

    public static ConversationModel ToModel(this Conversation entity)
    {
        ConversationModel model = new()
        {
            Id = Guid.Parse(entity.Id),
            UserId = Guid.Parse(entity.UserId),
            Language = entity.Language.ToString(),
            Mode = entity.Mode.ToString(),
            ScenarioId = string.IsNullOrEmpty(entity.ScenarioId) ? null : Guid.Parse(entity.ScenarioId),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsActive = entity.IsActive
        };

        return model;
    }

    public static IEnumerable<Conversation> ToEntities(this IEnumerable<ConversationModel> models)
    {
        return models.Select(m => m.ToEntity());
    }
}
