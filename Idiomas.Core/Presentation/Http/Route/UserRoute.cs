
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Route;
using Idiomas.Core.Presentation.DTO.User;
using Idiomas.Core.Presentation.Http.Validator;
using Idiomas.Core.Presentation.Http.Validator.User;

namespace Idiomas.Core.Presentation.Http.Route;

public class UserRoute(IUserController controller) : IRoute
{
    private readonly IUserController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/user", _controller.SaveUser)
            .Produces<CreateUserResponseDTO>(StatusCodes.Status201Created)
            .WithValidation<CreateUserValidator, CreateUserDTO>();

        app.MapPut("/user/{userid}", _controller.UpdateUser)
            .Produces<UpdateUserResponseDTO>(StatusCodes.Status200OK)
            .RequireAuthorization()
            .WithValidation<UpdateUserValidator, UpdateUserDTO>();
    }
}