using IdiomasAPI.Source.Application.DTO.User;
using IdiomasAPI.Source.Application.UseCase.UserCase;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Presentation.DTO.User;
using IdiomasAPI.Source.Presentation.Mapper;

namespace IdiomasAPI.Source.Presentation.Http.Controller;

public class UserController() : IUserController
{
    public async Task<IResult> SaveUser(CreateUserDTO dto, CreateUser useCase)
    {
        User user = await useCase.Execute(dto);

        CreateUserResponseDTO response = new() { User = user.ToResponseDTO() };

        return TypedResults.Created($"/user/{user.Id}", response);
    }
}