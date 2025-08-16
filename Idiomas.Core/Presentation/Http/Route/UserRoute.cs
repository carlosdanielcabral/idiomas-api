
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Route;
using Idiomas.Core.Presentation.DTO.User;

namespace Idiomas.Core.Presentation.Http.Route;

public class UserRoute(IUserController controller) : IRoute
{
    private readonly IUserController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/user", _controller.SaveUser)
            .Produces<CreateUserResponseDTO>(StatusCodes.Status201Created);

        app.MapPut("/user/{userid}", _controller.UpdateUser)
            .Produces<UpdateUserResponseDTO>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }
}