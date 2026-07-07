using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.UseCase.UserCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Presentation.DTO.User;
using Idiomas.Core.Presentation.Mapper;
using Idiomas.Core.Presentation.Extensions;
using System.Security.Claims;

namespace Idiomas.Core.Presentation.Http.Controller;

public class UserController() : IUserController
{
    public async Task<IResult> SaveUser(CreateUserDTO dto, CreateUser useCase)
    {
        User user = await useCase.Execute(dto);

        CreateUserResponseDTO response = new() { User = user.ToResponseDTO() };

        return TypedResults.Created($"/user/{user.Id}", response);
    }

    public async Task<IResult> UpdateUser(UpdateUserDTO dto, ClaimsPrincipal user, UpdateUser useCase)
    {
        var userIdString = user.GetUserId().ToString();

        User updatedUser = await useCase.Execute(userIdString, dto);

        UpdateUserResponseDTO response = new() { User = updatedUser.ToResponseDTO() };

        return TypedResults.Ok(response);
    }
}