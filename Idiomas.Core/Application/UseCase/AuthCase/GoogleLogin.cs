using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
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
            return await this.LinkGoogleCredential(existingUser.Id, payload);
        }

        return await this.CreateNewGoogleUser(payload);
    }

    private async Task<User> LinkGoogleCredential(string userId, GoogleTokenPayload payload)
    {
        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        UserCredential credential = new(
            UUIDGenerator.Generate(),
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

        User user = new(UUIDGenerator.Generate(), payload.Name, payload.Email, true);

        User createdUser = await this._userRepository.Insert(user);

        UserCredential credential = new(
            UUIDGenerator.Generate(),
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
