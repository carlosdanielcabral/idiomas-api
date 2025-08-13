using System.Security.Claims;
using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Application.UseCase.DictionaryCase;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Presentation.DTO.Dictionary;
using IdiomasAPI.Source.Presentation.Extensions;
using IdiomasAPI.Source.Presentation.Mapper;

namespace IdiomasAPI.Source.Presentation.Http.Controller;

public class DictionaryController() : IDictionaryController
{
    public async Task<IResult> SaveWord(CreateWordDTO dto, ClaimsPrincipal user, CreateWord useCase)
    {
        string userIdString = user.GetUserId().ToString();

        Word word = await useCase.Execute(dto, userIdString);

        CreateWordResponseDTO response = new() { Word = word.ToResponseDTO() };

        return TypedResults.Created($"/dictionary/word/{word.Id}", response);
    }

    public async Task<IResult> ListWords(ClaimsPrincipal user, ListWords useCase)
    {
        string userIdString = user.GetUserId().ToString();

        IEnumerable<Word> words = await useCase.Execute(userIdString);

        ListWordsResponseDTO response = new() { Words = words.ToResponseDTO() };

        return TypedResults.Ok(response);
    }

    public async Task<IResult> UpdateWord(string id, UpdateWordDTO dto, ClaimsPrincipal user, UpdateWord useCase)
    {
        string userIdString = user.GetUserId().ToString();

        Word updatedWord = await useCase.Execute(id, dto, userIdString);

        UpdateWordResponseDTO response = new() { Word = updatedWord.ToResponseDTO() };

        return TypedResults.Ok(response);
    }
    
    public async Task<IResult> DeleteWord(string id, ClaimsPrincipal user, DeleteWord useCase)
    {
        string userIdString = user.GetUserId().ToString();

        await useCase.Execute(id, userIdString);

        return TypedResults.NoContent();
    }
}