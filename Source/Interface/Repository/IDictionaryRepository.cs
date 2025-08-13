using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Interface.Repository;

public interface IDictionaryRepository
{
    public Task Insert(Word user);
    public Task<IEnumerable<Word>> GetAll(string userId);
    public Task<Word?> GetById(string id);
    public Task<Word?> GetByWord(string word, string userId);
    public Task<Word> Update(Word word);
}