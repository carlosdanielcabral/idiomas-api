using IdiomasAPI.Source.Interface.Repository;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Application.Mapper;
using IdiomasAPI.Source.Application.Error;
using System.Net;

namespace IdiomasAPI.Source.Application.UseCase.DictionaryCase;

public class CreateWord(IDictionaryRepository dictionaryRepository)
{
    private IDictionaryRepository _dictionaryRepository = dictionaryRepository;

    public async Task<Word> Execute(CreateWordDTO dto, string userId)
    {
        await this.ValidateWord(dto, userId);
    
        return await this._dictionaryRepository.Insert(dto.ToEntity(userId));
    }

    private async Task ValidateWord(CreateWordDTO dto, string userId)
    {
        Word? previousWord = await this._dictionaryRepository.GetByWord(dto.Word, userId);

        if (previousWord != null)
        {
            throw new ApiException("Palavra já cadastrada", HttpStatusCode.Conflict);
        }
    }
}