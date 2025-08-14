
using Idiomas.Source.Interface.Controller;
using Idiomas.Source.Interface.Route;
using Idiomas.Source.Presentation.DTO.Auth;

namespace Idiomas.Source.Presentation.Http.Route;

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