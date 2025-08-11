using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Context;
using IdiomasAPI.Source.Infrastructure.Database.Mapper;
using IdiomasAPI.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace IdiomasAPI.Source.Infrastructure.Database.Repository;

public class UserRepository(UserContext database) : IUserRepository
{
    private readonly UserContext _database = database;

    public async Task Insert(User user)
    {
        this._database.Add(user.ToModel());

        await this._database.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        var models = await this._database.User.ToListAsync();

        return models.ToEntities();
    }
}
