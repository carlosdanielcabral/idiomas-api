using System.Net;

using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Presentation.DTO.Conversation;

namespace Idiomas.Core.Presentation.Http.Validator.Conversation;

public class StartConversationValidator : IValidator<CreateConversationRequestDTO>
{
    public void Validate(CreateConversationRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Language))
        {
            throw new ApiException("Idioma é obrigatório", HttpStatusCode.BadRequest);
        }

        if (dto.Mode == ConversationMode.Guided && string.IsNullOrWhiteSpace(dto.ScenarioId))
        {
            throw new ApiException("ScenarioId é obrigatório para conversas guiadas", HttpStatusCode.BadRequest);
        }
    }
}
