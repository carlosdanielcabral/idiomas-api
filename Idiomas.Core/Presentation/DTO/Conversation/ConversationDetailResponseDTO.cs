using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Presentation.DTO.Conversation;

public class ConversationDetailResponseDTO
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

public class MessageResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessageRole Role { get; set; }
    public List<CorrectionResponseDTO> Corrections { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CorrectionResponseDTO
{
    public string OriginalFragment { get; set; } = string.Empty;
    public string SuggestedFragment { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public ErrorType Type { get; set; }
}
