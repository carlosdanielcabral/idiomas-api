using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.Mapper;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.UserCase;

public class CreateUser(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash,
    ITransactionManager transactionManager)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IHash _hash = hash;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<User> Execute(CreateUserDTO dto)
    {
        await this.ValidateUser(dto);

        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        User user = await this.CreateUserEntity(dto);

        await this.CreateLocalCredential(dto, user.Id);

        await transaction.CommitAsync();

        return user;
    }

    private async Task<User> CreateUserEntity(CreateUserDTO dto)
    {
        User user = dto.ToEntity();

        return await this._userRepository.Insert(user);
    }

    private async Task CreateLocalCredential(CreateUserDTO dto, string userId)
    {
        string passwordHash = this._hash.Hash(dto.Password);

        UserCredential credential = dto.ToCredentialEntity(userId, passwordHash);

        await this._userCredentialRepository.Insert(credential);
    }

    private async Task ValidateUser(CreateUserDTO dto)
    {
        User? existingUser = await this._userRepository.GetByEmail(dto.Email);

        if (existingUser != null)
        {
            throw new ApiException("E-mail já cadastrado", HttpStatusCode.Conflict);
        }
    }
}
