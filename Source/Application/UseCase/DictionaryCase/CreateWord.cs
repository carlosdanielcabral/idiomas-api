using IdiomasAPI.Source.Interface.Repository;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Infrastructure.Helper;

namespace IdiomasAPI.Source.Application.UseCase.DictionaryCase;

public class CreateWord(IDictionaryRepository dictionaryRepository)
{
    private IDictionaryRepository _dictionaryRepository = dictionaryRepository;

    public async Task<Word> Execute(CreateWordDTO dto, string userId)
    {
        Word word = new(
            UUIDGenerator.Generate(),
            dto.Word,
            dto.Ipa,
            userId,
            [.. dto.Meanings.Select(m => new Meaning(
                UUIDGenerator.Generate(),
                m.Meaning,
                m.Example
            ))]
        );

        await this._dictionaryRepository.Insert(word);

        return word;
    }
}