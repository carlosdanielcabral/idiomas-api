using Idiomas.Core.Infrastructure.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Context;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<UserModel> User { get; set; }
    public DbSet<WordModel> Word { get; set; }
    public DbSet<MeaningModel> Meaning { get; set; }
    public DbSet<FileModel> File { get; set; }
}