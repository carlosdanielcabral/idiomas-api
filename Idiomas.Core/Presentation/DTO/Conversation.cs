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

public class CreateConversationRequestDTO
{
    public string? Language { get; set; }
    public ConversationMode Mode { get; set; }
    public string? ScenarioId { get; set; }
}

public class ListScenariosRequestDTO
{
    public string? Language { get; set; }
}

public class SendMessageRequestDTO
{
    public string Content { get; set; } = string.Empty;
}
