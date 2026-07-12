using System.Text.RegularExpressions;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error.Validation;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public partial class ForgotPasswordValidator : IValidator<ForgotPasswordDTO>
{
    public void Validate(ForgotPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new FieldRequiredException("email");
        }

        if (!EmailRegex().IsMatch(dto.Email))
        {
            throw new FieldInvalidException("email");
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
