using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Presentation.DTO.Conversation;

public class ConversationListResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public Language Language { get; set; }
    public ConversationMode Mode { get; set; }
    public string? ScenarioTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public int MessageCount { get; set; }
}

public class ConversationListWrapperDTO
{
    public List<ConversationListResponseDTO> Conversations { get; set; } = new();
}
