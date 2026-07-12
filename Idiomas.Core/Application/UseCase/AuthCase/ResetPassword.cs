using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error.Auth;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class ResetPassword(
    IPasswordResetTokenRepository tokenRepository,
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork)
{
    private readonly IPasswordResetTokenRepository _tokenRepository = tokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IHash _hash = hash;
    private readonly ITokenHasher _tokenHasher = tokenHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Execute(ResetPasswordDTO dto)
    {
        string tokenHash = this._tokenHasher.Hash(dto.Token);

        PasswordResetToken? token = await this._tokenRepository.GetByTokenHash(tokenHash);

        if (token is null || !token.IsValid)
        {
            throw new TokenInvalidOrExpiredException();
        }

        User? user = await this._userRepository.GetById(token.UserId.ToString());

        if (user == null)
        {
            throw new TokenInvalidOrExpiredException();
        }

        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(user.Id, AuthProvider.Local);

        if (credential == null)
        {
            throw new TokenInvalidOrExpiredException();
        }

        string passwordHash = this._hash.Hash(dto.NewPassword);

        credential.UpdatePasswordHash(passwordHash);

        await this._unitOfWork.ExecuteAsync(async () =>
        {
            await this._userCredentialRepository.Update(credential);

            await this._tokenRepository.MarkAsUsed(token);
        });
    }
}
