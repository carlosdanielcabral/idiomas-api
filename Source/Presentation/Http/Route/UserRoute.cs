using IdiomasAPI.Source.Interface.Route;
using IdiomasAPI.Source.Interface.Controller;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public class UserRoute : IUserRoute
{
    private readonly IUserController _userController;

    public UserRoute(IUserController userController)
    {
        _userController = userController;
    }

    public void Register(WebApplication app)
    {
        app.MapPost("/user", _userController.SaveUser);
    }
}