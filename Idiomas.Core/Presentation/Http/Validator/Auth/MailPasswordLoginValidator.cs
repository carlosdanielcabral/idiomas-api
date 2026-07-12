using System.Text.RegularExpressions;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Exceptions.Validation;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public partial class MailPasswordLoginValidator : IValidator<MailPasswordLoginDTO>
{
    public void Validate(MailPasswordLoginDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new FieldRequiredException("email");
        }

        if (!EmailRegex().IsMatch(dto.Email))
        {
            throw new FieldInvalidException("email");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new FieldRequiredException("password");
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
