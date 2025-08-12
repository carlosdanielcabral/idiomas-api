using IdiomasAPI.Source.Presentation.Http.Route;

namespace IdiomasAPI.Source.Presentation.Http;

public class Router(UserRoute userRoute, AuthRoute authRoute)
{
    private readonly UserRoute _userRoute = userRoute;
    private readonly AuthRoute _authRoute = authRoute;

    public void Register(WebApplication app)
    {
        app.MapGet("/", () => "Online");

        this._userRoute.Register(app);
        this._authRoute.Register(app);
    }
}