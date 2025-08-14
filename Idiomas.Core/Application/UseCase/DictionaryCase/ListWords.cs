using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Application.UseCase.DictionaryCase;

public class ListWords(IDictionaryRepository dictionaryRepository)
{
    private IDictionaryRepository _dictionaryRepository = dictionaryRepository;

    public async Task<IEnumerable<Word>> Execute(string userId)
    {
        return await this._dictionaryRepository.GetAll(userId);
    }
}