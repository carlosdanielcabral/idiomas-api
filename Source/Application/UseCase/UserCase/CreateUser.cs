using IdiomasAPI.Source.Application.DTO.User;
using IdiomasAPI.Source.Infrastructure.Helper;
using IdiomasAPI.Source.Interface.Repository;
using IdiomasAPI.Source.Interface.Service;
using IdiomasAPI.Source.Application.Error;
using System.Net;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Application.Mapper;

namespace IdiomasAPI.Source.Application.UseCase.UserCase;

public class CreateUser(IUserRepository userRepository, IHash hash)
{
    private IUserRepository _userRepository = userRepository;
    private IHash _hash = hash;

    public async Task<User> Execute(CreateUserDTO dto)
    {
        await this.ValidateUser(dto);

        User user = dto.ToEntity();
        user.Password = this._hash.Hash(dto.Password);

        return await this._userRepository.Insert(user);
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