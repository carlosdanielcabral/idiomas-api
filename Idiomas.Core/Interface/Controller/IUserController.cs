using System.Security.Claims;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.UseCase.UserCase;
using Idiomas.Core.Presentation.Http.Validator.User;

namespace Idiomas.Core.Interface.Controller;

public interface IUserController
{
    public Task<IResult> SaveUser(CreateUserDTO dto, CreateUserValidator validator, CreateUser useCase);
    public Task<IResult> UpdateUser(UpdateUserDTO dto, ClaimsPrincipal user, UpdateUserValidator validator, UpdateUser useCase);
}