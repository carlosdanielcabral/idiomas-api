using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Context;
using IdiomasAPI.Source.Infrastructure.Database.Mapper;
using IdiomasAPI.Source.Interface.Repository;

namespace IdiomasAPI.Source.Infrastructure.Database.Repository;

public class DictionaryRepository(ApplicationContext database) : IDictionaryRepository
{
    private readonly ApplicationContext _database = database;

    public async Task Insert(Word word)
    {
        await this._database.Word.AddAsync(word.ToModel());

        await this._database.SaveChangesAsync();
    }
}
