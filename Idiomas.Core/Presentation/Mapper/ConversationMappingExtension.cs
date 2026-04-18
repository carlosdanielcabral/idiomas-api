using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Presentation.DTO.Conversation;

namespace Idiomas.Core.Presentation.Mapper;

public static class ConversationMappingExtension
{
    public static ConversationResponseDTO ToResponseDTO(this Conversation conversation)
    {
        return new ConversationResponseDTO
        {
            Id = conversation.Id,
            Language = conversation.Language,
            Mode = conversation.Mode,
            ScenarioId = conversation.ScenarioId,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            IsActive = conversation.IsActive,
            Messages = conversation.Messages.Select(m => m.ToResponseDTO()).ToList()
        };
    }

    public static MessageResponseDTO ToResponseDTO(this Message message)
    {
        return new MessageResponseDTO
        {
            Id = message.Id,
            Content = message.Content,
            Role = message.Role,
            Corrections = message.Corrections.Select(c => c.ToResponseDTO()).ToList(),
            CreatedAt = message.CreatedAt
        };
    }

    public static CorrectionResponseDTO ToResponseDTO(this Correction correction)
    {
        return new CorrectionResponseDTO
        {
            OriginalFragment = correction.OriginalFragment,
            SuggestedFragment = correction.SuggestedFragment,
            Explanation = correction.Explanation,
            Type = correction.Type
        };
    }

    public static ConversationListResponseDTO ToListResponseDTO(this Conversation conversation, string? scenarioTitle = null)
    {
        return new ConversationListResponseDTO
        {
            Id = conversation.Id,
            Language = conversation.Language,
            Mode = conversation.Mode,
            ScenarioTitle = scenarioTitle,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            IsActive = conversation.IsActive,
            MessageCount = conversation.Messages.Count
        };
    }

    public static IEnumerable<ConversationResponseDTO> ToResponseDTO(this IEnumerable<Conversation> conversations)
    {
        return conversations.Select(c => c.ToResponseDTO());
    }

    public static IEnumerable<ConversationListResponseDTO> ToListResponseDTO(this IEnumerable<Conversation> conversations)
    {
        return conversations.Select(c => c.ToListResponseDTO());
    }
}
