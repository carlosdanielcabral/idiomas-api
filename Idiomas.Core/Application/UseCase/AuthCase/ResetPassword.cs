using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class ResetPassword(
    IPasswordResetTokenRepository tokenRepository,
    IUserRepository userRepository,
    IHash hash)
{
    private readonly IPasswordResetTokenRepository _tokenRepository = tokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHash _hash = hash;

    public async Task Execute(ResetPasswordDTO dto)
    {
        PasswordResetToken? token = await this._tokenRepository.GetByToken(dto.Token);

        if (token == null || token.IsExpired || token.IsUsed)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        User? user = await this._userRepository.GetById(token.UserId.ToString());

        if (user == null)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        user.Password = this._hash.Hash(dto.NewPassword);

        await this._userRepository.Update(user);

        await this._tokenRepository.MarkAsUsed(token);
    }
}
