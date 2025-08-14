using IdiomasAPI.Source.Interface.Repository;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Application.Error;
using System.Net;

namespace IdiomasAPI.Source.Application.UseCase.DictionaryCase;

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
            throw new ApiException("Palavra não encontrada", HttpStatusCode.NotFound);
        }

        if (word.UserId != userId)
        {
            throw new ApiException("Você não tem permissão para deletar esta palavra", HttpStatusCode.Forbidden);
        }
    }
}