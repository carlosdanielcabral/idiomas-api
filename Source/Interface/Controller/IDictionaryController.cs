using System.Security.Claims;
using IdiomasAPI.Source.Application.DTO.Dictionary;

namespace IdiomasAPI.Source.Interface.Controller;

public interface IDictionaryController
{
    public Task<IResult> SaveWord(CreateWordDTO dto, ClaimsPrincipal user);
}