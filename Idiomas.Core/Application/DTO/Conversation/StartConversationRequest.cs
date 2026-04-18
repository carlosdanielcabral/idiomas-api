using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Application.DTO.Conversation;

public record StartConversationRequest(Language Language, ConversationMode Mode, string? ScenarioId);
