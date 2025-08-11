namespace IdiomasAPI.Source.Infrastructure.Http;

class App
{
    private WebApplication _app;
    private Router _router;

    public App(
        WebApplication? app = null,
        Router? router = null
    )
    {
        _app = app ?? WebApplication.CreateBuilder().Build();
        _router = router ?? new Router();

        this.Config();
    }

    private void Config()
    {
        this._router.Register(this._app);        
    }

    public void Start()
    {
        this._app.Run();
    }
}