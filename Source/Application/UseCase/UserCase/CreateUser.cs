using Idiomas.Source.Application.DTO.User;
using Idiomas.Source.Infrastructure.Helper;
using Idiomas.Source.Interface.Repository;
using Idiomas.Source.Interface.Service;
using Idiomas.Source.Application.Error;
using System.Net;
using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Application.Mapper;

namespace Idiomas.Source.Application.UseCase.UserCase;

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