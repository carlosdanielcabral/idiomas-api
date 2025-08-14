
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Route;
using Idiomas.Core.Presentation.DTO.Dictionary;

namespace Idiomas.Core.Presentation.Http.Route;

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

        dictionary.MapPut("/word/{id}", _controller.UpdateWord)
            .Produces<UpdateWordResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized);

        dictionary.MapDelete("/word/{id}", _controller.DeleteWord)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}