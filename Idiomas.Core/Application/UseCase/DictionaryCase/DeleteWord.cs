using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Application.Exceptions.Dictionary;

namespace Idiomas.Core.Application.UseCase.DictionaryCase;

public class DeleteWord(IDictionaryRepository dictionaryRepository)
{
    private IDictionaryRepository _dictionaryRepository = dictionaryRepository;

    public async Task Execute(string id, string userId)
    {
        await this.ValidateWord(id, userId);

        await this._dictionaryRepository.Delete(id);
    }

    private async Task ValidateWord(string id, string userId)
    {
        Word? word = await this._dictionaryRepository.GetById(id);

        if (word is null)
        {
            throw new WordNotFoundException();
        }

        if (word.UserId != userId)
        {
            throw new WordAccessDeniedException();
        }
    }
}
