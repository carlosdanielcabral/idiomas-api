
using Idiomas.Source.Interface.Controller;
using Idiomas.Source.Interface.Route;
using Idiomas.Source.Presentation.DTO.User;

namespace Idiomas.Source.Presentation.Http.Route;

public class UserRoute(IUserController controller) : IRoute
{
    private readonly IUserController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/user", _controller.SaveUser)
            .Produces<CreateUserResponseDTO>(StatusCodes.Status201Created);
    }
}