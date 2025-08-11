using IdiomasAPI.Source.Presentation.Http.Controller;
using IdiomasAPI.Source.Interface.Route;
using IdiomasAPI.Source.Interface.Controller;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public class UserRoute(IUserController userController) : IUserRoute
{
    private readonly IUserController _userController = userController;

    public void Register(WebApplication app)
    {
        app.MapPost("/user", this._userController.SaveUser);
    }
}