using Idiomas.Source.Domain.Entity;

namespace Idiomas.Source.Interface.Repository;

public interface IUserRepository
{
    public Task<User> Insert(User user);
    public Task<User?> GetByEmail(string email);
}