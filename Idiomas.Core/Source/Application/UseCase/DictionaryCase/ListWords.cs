using Idiomas.Source.Interface.Repository;
using Idiomas.Source.Domain.Entity;

namespace Idiomas.Source.Application.UseCase.DictionaryCase;

public class ListWords(IDictionaryRepository dictionaryRepository)
{
    private IDictionaryRepository _dictionaryRepository = dictionaryRepository;

    public async Task<IEnumerable<Word>> Execute(string userId)
    {
        return await this._dictionaryRepository.GetAll(userId);
    }
}