using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.Mapper;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.UserCase;

public class UpdateUser(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash,
    ITransactionManager transactionManager)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IHash _hash = hash;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<User> Execute(string userId, UpdateUserDTO dto)
    {
        await this.ValidateUser(userId, dto);

        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        User updatedUser = await this.UpdateUserProfile(userId, dto);

        if (!string.IsNullOrEmpty(dto.Password))
        {
            await this.UpdateUserPassword(userId, dto.Password);
        }

        await transaction.CommitAsync();

        return updatedUser;
    }

    private async Task<User> UpdateUserProfile(string userId, UpdateUserDTO dto)
    {
        User updatedUser = dto.ToEntity(userId);

        return await this._userRepository.Update(updatedUser);
    }

    private async Task UpdateUserPassword(string userId, string password)
    {
        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(userId, AuthProvider.Local);

        if (credential is null)
        {
            throw new ApiException("Usuário não possui credencial local", HttpStatusCode.BadRequest);
        }

        string passwordHash = this._hash.Hash(password);
        credential.UpdatePasswordHash(passwordHash);

        await this._userCredentialRepository.Update(credential);
    }

    private async Task ValidateUser(string userId, UpdateUserDTO dto)
    {
        User? user = await this._userRepository.GetById(userId);

        if (user is null)
        {
            throw new ApiException("Usuário não encontrado", HttpStatusCode.NotFound);
        }

        User? userWithEmail = await this._userRepository.GetByEmail(dto.Email);

        if (userWithEmail is not null && userWithEmail.Id != userId)
        {
            throw new ApiException("E-mail já cadastrado por outro usuário", HttpStatusCode.Conflict);
        }
    }
}
