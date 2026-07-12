using Idiomas.Core.Exceptions.Validation;
using Idiomas.Core.Presentation.DTO.Conversation;

namespace Idiomas.Core.Presentation.Http.Validator.Conversation;

public class SendMessageValidator : IValidator<SendMessageRequestDTO>
{
    private const int MAXIMUM_CONTENT_LENGTH = 4000;

    public void Validate(SendMessageRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            throw new FieldRequiredException("content");
        }

        if (dto.Content.Length > MAXIMUM_CONTENT_LENGTH)
        {
            throw new StringTooLongException("content", MAXIMUM_CONTENT_LENGTH);
        }
    }
}
