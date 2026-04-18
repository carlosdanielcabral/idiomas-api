using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Infrastructure.Service.LLM;

// Internal models for Gemini service implementation
internal record MessageContext(MessageRole Role, string Content);

internal record LLMRequest(
    string UserMessage,
    Language Language,
    ConversationMode Mode,
    string? ScenarioDescription);

internal record LLMResponse(
    string Content,
    List<CorrectionDetail> Corrections)
{
    public LLMResponse() : this(string.Empty, new List<CorrectionDetail>()) { }
}

internal record CorrectionDetail(
    string OriginalFragment,
    string SuggestedFragment,
    string Explanation,
    ErrorType Type);
