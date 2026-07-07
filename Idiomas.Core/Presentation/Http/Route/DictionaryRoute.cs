
using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Route;
using Idiomas.Core.Presentation.DTO.Dictionary;
using Idiomas.Core.Presentation.Http.Validator;
using Idiomas.Core.Presentation.Http.Validator.Dictionary;

namespace Idiomas.Core.Presentation.Http.Route;

public class DictionaryRoute(IDictionaryController controller) : IRoute
{
    private readonly IDictionaryController _controller = controller;

    public void Register(WebApplication app)
    {
        var dictionary = app.MapGroup("/dictionary").RequireAuthorization();

        dictionary.MapPost("/word", _controller.SaveWord)
            .Produces<CreateWordResponseDTO>(StatusCodes.Status201Created)
            .WithValidation<CreateWordValidator, CreateWordDTO>();

        dictionary.MapGet("/word", _controller.ListWords)
            .Produces<ListWordsResponseDTO>(StatusCodes.Status200OK);

        dictionary.MapPut("/word/{id}", _controller.UpdateWord)
            .Produces<UpdateWordResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithValidation<UpdateWordValidator, UpdateWordDTO>();

        dictionary.MapDelete("/word/{id}", _controller.DeleteWord)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}