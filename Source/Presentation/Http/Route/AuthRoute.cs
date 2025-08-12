
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Interface.Route;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public class AuthRoute(IAuthController controller) : IRoute
{
    private readonly IAuthController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/auth/login", this._controller.MailPasswordLogin);
    }
}