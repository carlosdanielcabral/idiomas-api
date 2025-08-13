using IdiomasAPI.Source.Application.DTO.User;
using IdiomasAPI.Source.Application.UseCase.UserCase;

namespace IdiomasAPI.Source.Interface.Controller;

public interface IUserController
{
    public Task<IResult> SaveUser(CreateUserDTO dto, CreateUser useCase);
}