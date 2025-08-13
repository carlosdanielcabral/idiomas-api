using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Interface.Repository;

public interface IDictionaryRepository
{
    public Task Insert(Word user);
    public Task<IEnumerable<Word>> GetAll(string userId);
}