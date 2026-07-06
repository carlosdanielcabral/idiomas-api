using System.Net;
using System.Text.RegularExpressions;

using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public partial class MailPasswordLoginValidator
{
    public void Validate(MailPasswordLoginDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ApiException("Email é obrigatório", HttpStatusCode.BadRequest);
        }

        if (!EmailRegex().IsMatch(dto.Email))
        {
            throw new ApiException("Email inválido", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new ApiException("Senha é obrigatória", HttpStatusCode.BadRequest);
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
