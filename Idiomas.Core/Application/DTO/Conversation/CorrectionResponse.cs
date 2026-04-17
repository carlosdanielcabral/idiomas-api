using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Application.DTO.Conversation;

public record CorrectionResponse(string OriginalFragment, string SuggestedFragment, string Explanation, ErrorType Type);
