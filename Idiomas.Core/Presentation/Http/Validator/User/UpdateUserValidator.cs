using System.Net;
using System.Text.RegularExpressions;

using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error;

namespace Idiomas.Core.Presentation.Http.Validator.User;

public partial class UpdateUserValidator : IValidator<UpdateUserDTO>
{
    private const int MINIMUM_PASSWORD_LENGTH = 8;

    public void Validate(UpdateUserDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ApiException("Nome é obrigatório", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ApiException("Email é obrigatório", HttpStatusCode.BadRequest);
        }

        if (!EmailRegex().IsMatch(dto.Email))
        {
            throw new ApiException("Email inválido", HttpStatusCode.BadRequest);
        }

        if (!string.IsNullOrEmpty(dto.Password) && dto.Password.Length < MINIMUM_PASSWORD_LENGTH)
        {
            throw new ApiException($"Senha deve ter pelo menos {MINIMUM_PASSWORD_LENGTH} caracteres", HttpStatusCode.BadRequest);
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
