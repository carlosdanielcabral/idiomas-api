using System.Security.Claims;
using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Application.UseCase.DictionaryCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Api.Interface.Controller;
using Idiomas.Api.DTO.Dictionary;
using Idiomas.Api.Extensions;
using Idiomas.Api.Mapper;

namespace Idiomas.Api.Http.Controller;

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