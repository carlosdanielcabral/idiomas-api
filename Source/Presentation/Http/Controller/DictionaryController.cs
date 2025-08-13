using System.Security.Claims;
using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Application.UseCase.DictionaryCase;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Presentation.DTO.Dictionary;
using IdiomasAPI.Source.Presentation.Extensions;
using IdiomasAPI.Source.Presentation.Mapper;

namespace IdiomasAPI.Source.Presentation.Http.Controller;

public class DictionaryController(CreateWord createWordUseCase) : IDictionaryController
{
    private readonly CreateWord _createWordUseCase = createWordUseCase;

    public async Task<IResult> SaveWord(CreateWordDTO dto, ClaimsPrincipal user)
    {
        string userIdString = user.GetUserId().ToString();

        Word word = await this._createWordUseCase.Execute(dto, userIdString);

        CreateWordResponseDTO response = new(){ Word = word.ToResponseDTO() };

        return TypedResults.Created($"/dictionary/word/{word.Id}", response);
    }

    public async Task<IResult> ListWords(ClaimsPrincipal user, ListWords listWordsUseCase)
    {
        string userIdString = user.GetUserId().ToString();

        IEnumerable<Word> words = await listWordsUseCase.Execute(userIdString);

        ListWordsResponseDTO response = new() { Words = words.ToResponseDTO() };

        return TypedResults.Ok(response);
    }
}