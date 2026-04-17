using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Idiomas.Core.Infrastructure.Database.Context;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=Idiomas;User Id=sa;Password=R00t@R00t;TrustServerCertificate=True;");

        return new ApplicationContext(optionsBuilder.Options);
    }
}
