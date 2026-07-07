using System.Net;

using Idiomas.Core.Application.Error;
using Idiomas.Core.Presentation.DTO.Conversation;

namespace Idiomas.Core.Presentation.Http.Validator.Conversation;

public class SendMessageValidator : IValidator<SendMessageRequestDTO>
{
    private const int MAXIMUM_CONTENT_LENGTH = 4000;

    public void Validate(SendMessageRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            throw new ApiException("Conteúdo da mensagem é obrigatório", HttpStatusCode.BadRequest);
        }

        if (dto.Content.Length > MAXIMUM_CONTENT_LENGTH)
        {
            throw new ApiException($"Conteúdo da mensagem deve ter no máximo {MAXIMUM_CONTENT_LENGTH} caracteres", HttpStatusCode.BadRequest);
        }
    }
}
