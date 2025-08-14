
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Route;
using Idiomas.Core.Presentation.DTO.Auth;

namespace Idiomas.Core.Presentation.Http.Route;

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