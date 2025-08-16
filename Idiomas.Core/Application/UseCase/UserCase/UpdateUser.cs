using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error;
using System.Net;
using Idiomas.Core.Application.Mapper;

namespace Idiomas.Core.Application.UseCase.UserCase;

public class UpdateUser(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<User> Execute(string userId, UpdateUserDTO dto)
    {
        await this.ValidateUser(userId, dto);

        var updatedUser = dto.ToEntity(userId);
        
        return await _userRepository.Update(updatedUser);
    }

    private async Task ValidateUser(string userId, UpdateUserDTO dto)
    {
        var user = await _userRepository.GetById(userId);

        if (user is null)
        {
            throw new ApiException("Usuário não encontrado", HttpStatusCode.NotFound);
        }

        var userWithEmail = await _userRepository.GetByEmail(dto.Email);

        if (userWithEmail is not null && userWithEmail.Id != userId)
        {
            throw new ApiException("E-mail já cadastrado por outro usuário", HttpStatusCode.Conflict);
        }
    }
}
