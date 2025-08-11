namespace IdiomasAPI.Source.Infrastructure.Http;

class Router
{
    public void Register(WebApplication app)
    {
        app.MapGet("/", () => "Online");
    }
}