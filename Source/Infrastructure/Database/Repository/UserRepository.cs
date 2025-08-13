using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Context;
using IdiomasAPI.Source.Infrastructure.Database.Mapper;
using IdiomasAPI.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace IdiomasAPI.Source.Infrastructure.Database.Repository;

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
        var models = await this._database.User.ToListAsync();
        return models.ToEntities();
    }

    public async Task<User?> GetByEmail(string email)
    {
        var model = await this._database.User.FirstOrDefaultAsync(u => u.Email == email);

        return model?.ToEntity();
    }
}
