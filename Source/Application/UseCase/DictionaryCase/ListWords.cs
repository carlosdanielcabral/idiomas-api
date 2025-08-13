using IdiomasAPI.Source.Interface.Repository;
using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Application.UseCase.DictionaryCase;

public class ListWords(IDictionaryRepository dictionaryRepository)
{
    private IDictionaryRepository _dictionaryRepository = dictionaryRepository;

    public async Task<IEnumerable<Word>> Execute(string userId)
    {
        return await this._dictionaryRepository.GetAll(userId);
    }
}