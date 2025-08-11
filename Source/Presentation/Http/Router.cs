using IdiomasAPI.Source.Interface.Route;

namespace IdiomasAPI.Source.Presentation.Http;

public class Router(IUserRoute userRoute)
{
    private readonly IUserRoute _userRoute = userRoute;

    public void Register(WebApplication app)
    {
        app.MapGet("/", () => "Online");

        this._userRoute.Register(app);
    }
}