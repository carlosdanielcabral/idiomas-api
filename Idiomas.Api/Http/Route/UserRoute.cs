
using Idiomas.Api.Interface.Controller;
using Idiomas.Api.Interface.Route;
using Idiomas.Api.DTO.User;

namespace Idiomas.Api.Http.Route;

public class UserRoute(IUserController controller) : IRoute
{
    private readonly IUserController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/user", _controller.SaveUser)
            .Produces<CreateUserResponseDTO>(StatusCodes.Status201Created);
    }
}