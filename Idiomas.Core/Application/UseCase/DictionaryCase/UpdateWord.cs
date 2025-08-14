using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Application.Error;
using System.Net;
using Idiomas.Core.Application.Mapper;

namespace Idiomas.Core.Application.UseCase.DictionaryCase;

public class UpdateWord(IDictionaryRepository dictionaryRepository)
{
    private IDictionaryRepository _dictionaryRepository = dictionaryRepository;

    public async Task<Word> Execute(string id, UpdateWordDTO dto, string userId)
    {
        await this.ValidateWord(id, dto, userId);

        return await this._dictionaryRepository.Update(dto.ToEntity(id, userId));
    }
    
    private async Task ValidateWord(string id, UpdateWordDTO dto, string userId)
    {
        Word? previousWord = await this._dictionaryRepository.GetById(id);

        if (previousWord is null)
        {
            throw new ApiException("Palavra não encontrada", HttpStatusCode.NotFound);
        }

        if (previousWord.UserId != userId)
        {
            throw new ApiException("Você não tem permissão para atualizar esta palavra", HttpStatusCode.Forbidden);
        }

        bool isWordChanged = dto.Word != previousWord.Name;

        if (!isWordChanged)
        {
            return;
        }

        Word? existingWord = await this._dictionaryRepository.GetByWord(dto.Word, userId);

        if (existingWord is not null)
        {
            throw new ApiException("Palavra já cadastrada", HttpStatusCode.Conflict);
        }
    }   
}