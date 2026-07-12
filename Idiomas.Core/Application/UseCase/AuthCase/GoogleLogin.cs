using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.Google;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class GoogleLogin(
    IGoogleTokenVerifier tokenVerifier,
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    ITransactionManager transactionManager)
{
    private readonly IGoogleTokenVerifier _tokenVerifier = tokenVerifier;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<User> Execute(GoogleLoginDTO dto)
    {
        GoogleTokenPayload payload = await this._tokenVerifier.Verify(dto.IdToken);

        if (!payload.EmailVerified)
        {
            throw new ApiException("Email não verificado pelo Google", HttpStatusCode.Unauthorized);
        }

        UserCredential? credential = await this._userCredentialRepository
            .GetByExternalSubject(AuthProvider.Google, payload.Subject);

        if (credential != null)
        {
            User? user = await this._userRepository.GetById(credential.UserId);

            if (user == null)
            {
                throw new ApiException("Conta não encontrada", HttpStatusCode.Unauthorized);
            }

            return user;
        }

        User? existingUser = await this._userRepository.GetByEmail(payload.Email);

        if (existingUser != null)
        {
            if (!existingUser.CanLogin())
            {
                throw new ApiException("E-mail não verificado. Verifique sua caixa de entrada para ativar sua conta.", HttpStatusCode.Forbidden);
            }

            return await this.LinkGoogleCredential(existingUser.Id, payload);
        }

        return await this.CreateNewGoogleUser(payload);
    }

    private async Task<User> LinkGoogleCredential(string userId, GoogleTokenPayload payload)
    {
        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        UserCredential credential = UserCredential.Create(
            userId,
            AuthProvider.Google,
            null,
            payload.Subject
        );

        await this._userCredentialRepository.Insert(credential);

        await transaction.CommitAsync();

        User? user = await this._userRepository.GetById(userId);

        return user!;
    }

    private async Task<User> CreateNewGoogleUser(GoogleTokenPayload payload)
    {
        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        User user = User.Create(payload.Name, payload.Email, true);

        User createdUser = await this._userRepository.Insert(user);

        UserCredential credential = UserCredential.Create(
            createdUser.Id,
            AuthProvider.Google,
            null,
            payload.Subject
        );

        await this._userCredentialRepository.Insert(credential);

        await transaction.CommitAsync();

        return createdUser;
    }
}
