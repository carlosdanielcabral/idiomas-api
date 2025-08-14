using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class UserRepository(ApplicationContext database) : IUserRepository
{
    private readonly ApplicationContext _database = database;

    public async Task<User> Insert(User user)
    {
        this._database.User.Add(user.ToModel());

        await this._database.SaveChangesAsync();

        return user;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        List<UserModel> models = await this._database.User.ToListAsync();
        return models.ToEntities();
    }

    public async Task<User?> GetByEmail(string email)
    {
        UserModel? model = await this._database.User.FirstOrDefaultAsync(u => u.Email == email);

        return model?.ToEntity();
    }
}
