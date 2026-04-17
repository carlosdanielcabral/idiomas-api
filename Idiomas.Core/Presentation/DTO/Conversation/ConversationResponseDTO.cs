using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Presentation.DTO.Conversation;

public class ConversationResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public Language Language { get; set; }
    public ConversationMode Mode { get; set; }
    public string? ScenarioId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public List<MessageResponseDTO> Messages { get; set; } = new();
}

public class ScenarioResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
