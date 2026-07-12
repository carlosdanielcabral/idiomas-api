using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class VerifyEmailChange(
    IEmailChangeRequestRepository requestRepository,
    IUserRepository userRepository,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork)
{
    private readonly IEmailChangeRequestRepository _requestRepository = requestRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenHasher _tokenHasher = tokenHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Execute(string rawToken)
    {
        string tokenHash = this._tokenHasher.Hash(rawToken);

        EmailChangeRequest? request = await this._requestRepository.GetByTokenHash(tokenHash);

        if (request is null || !request.IsValid)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        User? user = await this._userRepository.GetById(request.UserId.ToString());

        if (user == null)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        user.UpdateEmail(request.NewEmail);

        await this._unitOfWork.ExecuteAsync(async () =>
        {
            await this._userRepository.Update(user);

            await this._requestRepository.MarkAsUsed(request);
        });
    }
}
