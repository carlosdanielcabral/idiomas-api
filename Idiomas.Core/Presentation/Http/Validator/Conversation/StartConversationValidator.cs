using Idiomas.Core.Application.Error.Validation;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Presentation.DTO.Conversation;

namespace Idiomas.Core.Presentation.Http.Validator.Conversation;

public class StartConversationValidator : IValidator<CreateConversationRequestDTO>
{
    public void Validate(CreateConversationRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Language))
        {
            throw new FieldRequiredException("language");
        }

        if (dto.Mode == ConversationMode.Guided && string.IsNullOrWhiteSpace(dto.ScenarioId))
        {
            throw new FieldRequiredException("scenarioId");
        }
    }
}
