using Idiomas.Source.Infrastructure.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Source.Infrastructure.Database.Context;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<UserModel> User { get; set; }
    public DbSet<WordModel> Word { get; set; }
    public DbSet<MeaningModel> Meaning { get; set; }
    public DbSet<FileModel> File { get; set; }
}