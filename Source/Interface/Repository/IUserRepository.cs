using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Interface.Repository;

public interface IUserRepository
{
    public Task<User> Insert(User user);
    public Task<User?> GetByEmail(string email);
}