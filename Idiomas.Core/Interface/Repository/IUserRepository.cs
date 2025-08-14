using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Interface.Repository;

public interface IUserRepository
{
    public Task<User> Insert(User user);
    public Task<User?> GetByEmail(string email);
}