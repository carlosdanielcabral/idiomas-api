
using Idiomas.Api.Interface.Controller;
using Idiomas.Api.Interface.Route;
using Idiomas.Api.DTO.Auth;

namespace Idiomas.Api.Http.Route;

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