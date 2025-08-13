using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Context;
using IdiomasAPI.Source.Infrastructure.Database.Mapper;
using IdiomasAPI.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace IdiomasAPI.Source.Infrastructure.Database.Repository;

public class DictionaryRepository(ApplicationContext database) : IDictionaryRepository
{
    private readonly ApplicationContext _database = database;

    public async Task<Word> Insert(Word word)
    {
        await this._database.Word.AddAsync(word.ToModel());

        await this._database.SaveChangesAsync();

        return word;
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

    public async Task<Word?> GetById(string id)
    {
        Guid wordId = Guid.Parse(id);

        var model = await this._database.Word
            .Include(w => w.Meanings)
            .FirstOrDefaultAsync(w => w.Id == wordId);

        return model?.ToEntity();
    }

    public async Task<Word?> GetByWord(string word, string userId)
    {
        Guid userGuid = Guid.Parse(userId);

        var model = await this._database.Word
            .Include(w => w.Meanings)
            .FirstOrDefaultAsync(w => w.Word == word && w.UserId == userGuid);

        return model?.ToEntity();
    }

    public async Task<Word> Update(Word word)
    {
        this._database.Word.Update(word.ToModel());

        await this._database.SaveChangesAsync();

        return word;
    }
}
