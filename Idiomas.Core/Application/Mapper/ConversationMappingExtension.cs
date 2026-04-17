using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;

namespace Idiomas.Core.Application.Mapper;

public static class ConversationMappingExtension
{
    public static Conversation ToEntity(this StartConversationRequest dto, string userId, string? scenarioId)
    {
        return new Conversation(UUIDGenerator.Generate(), userId, dto.Language, dto.Mode, scenarioId);
    }
}
