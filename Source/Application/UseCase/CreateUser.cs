using IdiomasAPI.Source.Application.DTO.User;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Service;
using IdiomasAPI.Source.Interface.Repository;
using IdiomasAPI.Source.Interface.Service;
using IdiomasAPI.Source.Application.Error;
using System.Net;

namespace IdiomasAPI.Source.Application.UseCase;

public class CreateUser(IUserRepository userRepository, IHash hash)
{
    private IUserRepository _userRepository = userRepository;
    private IHash _hash = hash;

    public async Task<User> Execute(CreateUserDTO dto)
    {
        var existingUser = await this._userRepository.GetByEmail(dto.Email);

        if (existingUser != null)
        {
            throw new ApiException("E-mail já cadastrado", HttpStatusCode.Conflict);
        }

        User user = new(
            UUIDGenerator.Generate(),
            dto.Name,
            dto.Email,
            this._hash.Hash(dto.Password)
        );

        await this._userRepository.Insert(user);

        return user;
    }
}