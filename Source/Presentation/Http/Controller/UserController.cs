using IdiomasAPI.Source.Application.DTO.User;
using IdiomasAPI.Source.Application.UseCase.UserCase;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Interface.Controller;

namespace IdiomasAPI.Source.Presentation.Http.Controller;

public class UserController(CreateUser createUserUseCase) : IUserController
{
    private readonly CreateUser _createUserUseCase = createUserUseCase;

    public async Task<IResult> SaveUser(CreateUserDTO dto)
    {
        User user = await this._createUserUseCase.Execute(dto);

        return TypedResults.Created($"/user/{user.Id}", user);
    }
}