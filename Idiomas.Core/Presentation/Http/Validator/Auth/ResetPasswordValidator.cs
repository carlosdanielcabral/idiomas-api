using System.Net;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;

namespace Idiomas.Core.Presentation.Http.Validator.Auth;

public class ResetPasswordValidator : IValidator<ResetPasswordDTO>
{
    private const int MIN_PASSWORD_LENGTH = 8;

    public void Validate(ResetPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token))
        {
            throw new ApiException("Token é obrigatório", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            throw new ApiException("Nova senha é obrigatória", HttpStatusCode.BadRequest);
        }

        if (dto.NewPassword.Length < MIN_PASSWORD_LENGTH)
        {
            throw new ApiException($"A senha deve ter no mínimo {MIN_PASSWORD_LENGTH} caracteres", HttpStatusCode.BadRequest);
        }
    }
}
