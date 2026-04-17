using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Application.DTO.Conversation;

public record ConversationResponse(string Id, Language Language, ConversationMode Mode, string? ScenarioId, DateTime CreatedAt);
