using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error.Validation;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public class ResetPasswordValidator : IValidator<ResetPasswordDTO>
{
    private const int MIN_PASSWORD_LENGTH = 8;

    public void Validate(ResetPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token))
        {
            throw new FieldRequiredException("token");
        }

        if (string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            throw new FieldRequiredException("newPassword");
        }

        if (dto.NewPassword.Length < MIN_PASSWORD_LENGTH)
        {
            throw new StringTooShortException("newPassword", MIN_PASSWORD_LENGTH);
        }
    }
}
