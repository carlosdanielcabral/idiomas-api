
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Interface.Route;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public class DictionaryRoute(IDictionaryController controller) : IRoute
{
    private readonly IDictionaryController _controller = controller;

    public void Register(WebApplication app)
    {
        var dictionary = app.MapGroup("/dictionary").RequireAuthorization();

        dictionary.MapPost("/word", _controller.SaveWord);
    }
}