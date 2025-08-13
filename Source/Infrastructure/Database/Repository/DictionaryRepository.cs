using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Context;
using IdiomasAPI.Source.Infrastructure.Database.Mapper;
using IdiomasAPI.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace IdiomasAPI.Source.Infrastructure.Database.Repository;

public class DictionaryRepository(ApplicationContext database) : IDictionaryRepository
{
    private readonly ApplicationContext _database = database;

    public async Task Insert(Word word)
    {
        await this._database.Word.AddAsync(word.ToModel());

        await this._database.SaveChangesAsync();
    }

    public async Task<IEnumerable<Word>> GetAll(string userId)
    {
        Guid userGuid = Guid.Parse(userId);

        var models = await this._database.Word
            .Where(w => w.UserId == userGuid)
            .Include(w => w.Meanings)
            .ToListAsync();

        return models.ToEntities();
    }
}
