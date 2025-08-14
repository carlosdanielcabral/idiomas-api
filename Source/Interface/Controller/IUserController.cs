using Idiomas.Source.Application.DTO.User;
using Idiomas.Source.Application.UseCase.UserCase;

namespace Idiomas.Source.Interface.Controller;

public interface IUserController
{
    public Task<IResult> SaveUser(CreateUserDTO dto, CreateUser useCase);
}