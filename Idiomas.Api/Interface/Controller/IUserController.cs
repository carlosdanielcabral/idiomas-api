using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.UseCase.UserCase;

namespace Idiomas.Api.Interface.Controller;

public interface IUserController
{
    public Task<IResult> SaveUser(CreateUserDTO dto, CreateUser useCase);
}