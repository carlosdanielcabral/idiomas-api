using Idiomas.Api.Http.Route;

namespace Idiomas.Api.Http;

public class Router(UserRoute userRoute, AuthRoute authRoute, DictionaryRoute dictionaryRoute, FileRoute fileRoute)
{
    private readonly UserRoute _userRoute = userRoute;
    private readonly AuthRoute _authRoute = authRoute;
    private readonly DictionaryRoute _dictionaryRoute = dictionaryRoute;
    private readonly FileRoute _fileRoute = fileRoute;

    public void Register(WebApplication app)
    {
        app.MapGet("/", () => "Online");

        this._userRoute.Register(app);
        this._authRoute.Register(app);
        this._dictionaryRoute.Register(app);
        this._fileRoute.Register(app);
    }
}