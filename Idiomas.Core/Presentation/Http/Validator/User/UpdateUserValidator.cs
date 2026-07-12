using System.Text.RegularExpressions;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Exceptions.Validation;

namespace Idiomas.Core.Presentation.Http.Validator.User;

public partial class UpdateUserValidator : IValidator<UpdateUserDTO>
{
    private const int MINIMUM_PASSWORD_LENGTH = 8;

    public void Validate(UpdateUserDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new FieldRequiredException("name");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new FieldRequiredException("email");
        }

        if (!EmailRegex().IsMatch(dto.Email))
        {
            throw new FieldInvalidException("email");
        }

        if (!string.IsNullOrEmpty(dto.Password) && dto.Password.Length < MINIMUM_PASSWORD_LENGTH)
        {
            throw new StringTooShortException("password", MINIMUM_PASSWORD_LENGTH);
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
