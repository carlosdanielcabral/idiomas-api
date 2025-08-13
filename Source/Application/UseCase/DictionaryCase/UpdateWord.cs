using IdiomasAPI.Source.Interface.Repository;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Application.Error;
using System.Net;
using IdiomasAPI.Source.Application.Mapper;

namespace IdiomasAPI.Source.Application.UseCase.DictionaryCase;

public class UpdateWord(IDictionaryRepository dictionaryRepository)
{
    private IDictionaryRepository _dictionaryRepository = dictionaryRepository;

    public async Task<Word> Execute(UpdateWordDTO dto, string userId)
    {
        this.ValidateWord(dto, userId);

        return await this._dictionaryRepository.Update(dto.ToEntity(userId));
    }
    
    private async void ValidateWord(UpdateWordDTO dto, string userId)
    {
        Word? previousWord = await this._dictionaryRepository.GetById(dto.Id);

        if (previousWord is null)
        {
            throw new ApiException("Palavra não encontrada", HttpStatusCode.NotFound);
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