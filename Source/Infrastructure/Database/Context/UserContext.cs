using IdiomasAPI.Source.Infrastructure.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace IdiomasAPI.Source.Infrastructure.Database.Context;

public class UserContext(string connectionString) : DbContext
{
    private readonly string _connectionString = connectionString;

    public DbSet<UserModel> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(this._connectionString);
    }
}