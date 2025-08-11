using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Interface.Repository;

public interface IUserRepository
{
    public Task Insert(User user);
}