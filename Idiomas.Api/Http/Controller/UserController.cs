using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.UseCase.UserCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Api.Interface.Controller;
using Idiomas.Api.DTO.User;
using Idiomas.Api.Mapper;

namespace Idiomas.Api.Http.Controller;

public class UserController() : IUserController
{
    public async Task<IResult> SaveUser(CreateUserDTO dto, CreateUser useCase)
    {
        User user = await useCase.Execute(dto);

        CreateUserResponseDTO response = new() { User = user.ToResponseDTO() };

        return TypedResults.Created($"/user/{user.Id}", response);
    }
}