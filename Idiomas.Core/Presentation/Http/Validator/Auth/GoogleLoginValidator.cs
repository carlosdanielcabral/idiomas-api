using System.Net;

using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public class GoogleLoginValidator : IValidator<GoogleLoginDTO>
{
    public void Validate(GoogleLoginDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.IdToken))
        {
            throw new ApiException("ID Token é obrigatório", HttpStatusCode.BadRequest);
        }
    }
}
