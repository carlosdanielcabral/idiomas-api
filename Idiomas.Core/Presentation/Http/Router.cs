using Idiomas.Core.Presentation.Http.Route;

namespace Idiomas.Core.Presentation.Http;

public class Router(
    UserRoute userRoute,
    AuthRoute authRoute,
    DictionaryRoute dictionaryRoute,
    FileRoute fileRoute,
    ConversationRoute conversationRoute)
{
    private readonly UserRoute _userRoute = userRoute;
    private readonly AuthRoute _authRoute = authRoute;
    private readonly DictionaryRoute _dictionaryRoute = dictionaryRoute;
    private readonly FileRoute _fileRoute = fileRoute;
    private readonly ConversationRoute _conversationRoute = conversationRoute;

    public void Register(WebApplication app)
    {
        app.MapGet("/", () => "Online");

        this._userRoute.Register(app);
        this._authRoute.Register(app);
        this._dictionaryRoute.Register(app);
        this._fileRoute.Register(app);
        this._conversationRoute.Register(app);
    }
}