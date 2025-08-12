using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Interface.Repository;

public interface IDictionaryRepository
{
    Task Insert(Word user);
}