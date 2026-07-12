using System.Text.RegularExpressions;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Exceptions.Validation;

namespace Idiomas.Core.Presentation.Http.Validator.User;

public partial class CreateUserValidator : IValidator<CreateUserDTO>
{
    private const int MINIMUM_PASSWORD_LENGTH = 8;

    public void Validate(CreateUserDTO dto)
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

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new FieldRequiredException("password");
        }

        if (dto.Password.Length < MINIMUM_PASSWORD_LENGTH)
        {
            throw new StringTooShortException("password", MINIMUM_PASSWORD_LENGTH);
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
