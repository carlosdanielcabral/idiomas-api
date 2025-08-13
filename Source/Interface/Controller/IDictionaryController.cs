using System.Security.Claims;
using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Application.UseCase.DictionaryCase;

namespace IdiomasAPI.Source.Interface.Controller;

public interface IDictionaryController
{
    public Task<IResult> SaveWord(CreateWordDTO dto, ClaimsPrincipal user);
    public Task<IResult> ListWords(ClaimsPrincipal user, ListWords listWordsUseCase);
    public Task<IResult> UpdateWord(string id, UpdateWordDTO dto, ClaimsPrincipal user);
}