
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Interface.Route;
using IdiomasAPI.Source.Presentation.DTO.Dictionary;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public class DictionaryRoute(IDictionaryController controller) : IRoute
{
    private readonly IDictionaryController _controller = controller;

    public void Register(WebApplication app)
    {
        var dictionary = app.MapGroup("/dictionary").RequireAuthorization();

        dictionary.MapPost("/word", _controller.SaveWord)
            .Produces<CreateWordResponseDTO>(StatusCodes.Status201Created);
    
        dictionary.MapGet("/word", _controller.ListWords)
            .Produces<ListWordsResponseDTO>(StatusCodes.Status200OK);
    }
}