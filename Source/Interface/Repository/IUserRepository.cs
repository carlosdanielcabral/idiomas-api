using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Interface.Repository;

public interface IUserRepository
{
    Task Insert(User user);
    Task<User?> GetByEmail(string email);
}