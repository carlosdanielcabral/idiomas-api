using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Infrastructure.Database.Context;
using Idiomas.Source.Infrastructure.Database.Mapper;
using Idiomas.Source.Infrastructure.Database.Model;
using Idiomas.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Source.Infrastructure.Database.Repository;

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
