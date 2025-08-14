using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Context;
using IdiomasAPI.Source.Infrastructure.Database.Mapper;
using IdiomasAPI.Source.Infrastructure.Database.Model;
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

        List<WordModel> models = await this._database.Word
            .Where(w => w.UserId == userGuid)
            .Include(w => w.Meanings)
            .ToListAsync();

        return models.ToEntities();
    }

    public async Task<Word?> GetById(string id)
    {
        Guid wordId = Guid.Parse(id);

        WordModel? model = await this._database.Word
            .Include(w => w.Meanings)
            .FirstOrDefaultAsync(w => w.Id == wordId);

        return model?.ToEntity();
    }

    public async Task<Word?> GetByWord(string word, string userId)
    {
        Guid userGuid = Guid.Parse(userId);

        WordModel? model = await this._database.Word
            .Include(w => w.Meanings)
            .FirstOrDefaultAsync(w => w.Word == word && w.UserId == userGuid);

        return model?.ToEntity();
    }

    public async Task<Word> Update(Word updatedWord)
    {
        Guid wordId = Guid.Parse(updatedWord.Id);

        WordModel? outdatedWord = await this._database.Word
            .Include(w => w.Meanings)
            .FirstOrDefaultAsync(w => w.Id == wordId);

        if (outdatedWord is null)
        {
            throw new KeyNotFoundException($"Word with ID {wordId} not found.");
        }

        outdatedWord.Word = updatedWord.Name;
        outdatedWord.Ipa = updatedWord.Ipa;
        outdatedWord.UpdatedAt = DateTime.UtcNow;

        outdatedWord.Meanings.Clear();

        foreach (Meaning newMeaning in updatedWord.Meanings)
        {
            outdatedWord.Meanings.Add(newMeaning.ToModel());
        }

        await this._database.SaveChangesAsync();

        return outdatedWord.ToEntity();
    }
    
    public async Task Delete(string id)
    {
        Guid wordId = Guid.Parse(id);

        WordModel? model = await this._database.Word
            .Include(w => w.Meanings)
            .FirstOrDefaultAsync(w => w.Id == wordId);

        if (model is null)
        {
            throw new KeyNotFoundException($"Word with ID {wordId} not found.");
        }

        this._database.Word.Remove(model);

        await this._database.SaveChangesAsync();
    }
}
