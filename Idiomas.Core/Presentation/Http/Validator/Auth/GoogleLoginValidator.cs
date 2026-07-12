using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error.Validation;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public class GoogleLoginValidator : IValidator<GoogleLoginDTO>
{
    public void Validate(GoogleLoginDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.IdToken))
        {
            throw new FieldRequiredException("idToken");
        }
    }
}
