using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class VerifyEmail(
    IEmailVerificationTokenRepository tokenRepository,
    IUserRepository userRepository,
    ITokenHasher tokenHasher)
{
    private readonly IEmailVerificationTokenRepository _tokenRepository = tokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenHasher _tokenHasher = tokenHasher;

    public async Task Execute(string rawToken)
    {
        string tokenHash = this._tokenHasher.Hash(rawToken);

        EmailVerificationToken? token = await this._tokenRepository.GetByTokenHash(tokenHash);

        if (token == null || token.IsExpired || token.IsUsed)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        User? user = await this._userRepository.GetById(token.UserId.ToString());

        if (user == null)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        user.MarkEmailAsVerified();

        await this._userRepository.Update(user);

        await this._tokenRepository.MarkAsUsed(token);
    }
}
