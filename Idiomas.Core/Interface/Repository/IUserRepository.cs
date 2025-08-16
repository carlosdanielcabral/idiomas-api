using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Interface.Repository;

public interface IUserRepository
{
    public Task<User> Insert(User user);
    public Task<User?> GetByEmail(string email);
    public Task<User?> GetById(string id);
    public Task<User> Update(User user);
}