using Idiomas.Core.Application.Exceptions.Auth;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class VerifyEmail(
    IEmailVerificationTokenRepository tokenRepository,
    IUserRepository userRepository,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork)
{
    private readonly IEmailVerificationTokenRepository _tokenRepository = tokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenHasher _tokenHasher = tokenHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Execute(string rawToken)
    {
        string tokenHash = this._tokenHasher.Hash(rawToken);

        EmailVerificationToken? token = await this._tokenRepository.GetByTokenHash(tokenHash);

        if (token is null || !token.IsValid)
        {
            throw new TokenInvalidOrExpiredException();
        }

        User? user = await this._userRepository.GetById(token.UserId.ToString());

        if (user == null)
        {
            throw new TokenInvalidOrExpiredException();
        }

        user.MarkEmailAsVerified();

        await this._unitOfWork.ExecuteAsync(async () =>
        {
            await this._userRepository.Update(user);

            await this._tokenRepository.MarkAsUsed(token);
        });
    }
}
