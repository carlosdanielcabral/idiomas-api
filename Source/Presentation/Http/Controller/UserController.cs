using Idiomas.Source.Application.DTO.User;
using Idiomas.Source.Application.UseCase.UserCase;
using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Interface.Controller;
using Idiomas.Source.Presentation.DTO.User;
using Idiomas.Source.Presentation.Mapper;

namespace Idiomas.Source.Presentation.Http.Controller;

public class UserController() : IUserController
{
    public async Task<IResult> SaveUser(CreateUserDTO dto, CreateUser useCase)
    {
        User user = await useCase.Execute(dto);

        CreateUserResponseDTO response = new() { User = user.ToResponseDTO() };

        return TypedResults.Created($"/user/{user.Id}", response);
    }
}