using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Application.DTO.Conversation;

public record ConversationLLMResponse(
    string Content,
    List<CorrectionResponse> Corrections);
