using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Application.DTO.Conversation;

public record MessageResponse(string Id, string Content, MessageRole Role, List<CorrectionResponse> Corrections, DateTime CreatedAt);
