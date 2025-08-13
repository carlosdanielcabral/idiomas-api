
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Interface.Route;
using IdiomasAPI.Source.Presentation.DTO.Auth;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public class AuthRoute(IAuthController controller) : IRoute
{
    private readonly IAuthController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/auth/login", this._controller.MailPasswordLogin)
            .Produces<MailPasswordLoginResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}