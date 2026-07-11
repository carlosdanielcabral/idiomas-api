using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class ResetPassword(
    IPasswordResetTokenRepository tokenRepository,
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash)
{
    private readonly IPasswordResetTokenRepository _tokenRepository = tokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
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

        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(user.Id, AuthProvider.Local);

        if (credential == null)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        string passwordHash = this._hash.Hash(dto.NewPassword);
        credential.UpdatePasswordHash(passwordHash);

        await this._userCredentialRepository.Update(credential);

        await this._tokenRepository.MarkAsUsed(token);
    }
}
