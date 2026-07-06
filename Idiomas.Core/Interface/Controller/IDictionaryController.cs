using System.Security.Claims;
using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Application.UseCase.DictionaryCase;
using Idiomas.Core.Presentation.Http.Validator.Dictionary;

namespace Idiomas.Core.Interface.Controller;

public interface IDictionaryController
{
    public Task<IResult> SaveWord(CreateWordDTO dto, ClaimsPrincipal user, CreateWordValidator validator, CreateWord useCase);
    public Task<IResult> ListWords(ClaimsPrincipal user, ListWords useCase);
    public Task<IResult> UpdateWord(string id, UpdateWordDTO dto, ClaimsPrincipal user, UpdateWordValidator validator, UpdateWord useCase);
    public Task<IResult> DeleteWord(string id, ClaimsPrincipal user, DeleteWord useCase);
}