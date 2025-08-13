
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Interface.Route;
using IdiomasAPI.Source.Presentation.DTO.User;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public class UserRoute(IUserController controller) : IRoute
{
    private readonly IUserController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/user", _controller.SaveUser)
            .Produces<CreateUserResponseDTO>(StatusCodes.Status201Created);
    }
}