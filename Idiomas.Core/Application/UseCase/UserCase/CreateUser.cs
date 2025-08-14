using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Idiomas.Core.Application.Error;
using System.Net;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Application.Mapper;

namespace Idiomas.Core.Application.UseCase.UserCase;

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