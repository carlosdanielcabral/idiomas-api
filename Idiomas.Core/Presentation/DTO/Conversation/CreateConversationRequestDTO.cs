using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Presentation.DTO.Conversation;

public class CreateConversationRequestDTO
{
    public string? Language { get; set; }
    public ConversationMode Mode { get; set; }
    public string? ScenarioId { get; set; }
}
