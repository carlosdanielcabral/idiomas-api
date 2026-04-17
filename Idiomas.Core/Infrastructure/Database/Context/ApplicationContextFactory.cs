using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Idiomas.Core.Infrastructure.Database.Context;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    private const string DEFAULT_CONNECTION_STRING = "Server=database,1433;Database=Idiomas;User Id=sa;Password=R00t@R00t;TrustServerCertificate=True;";
    private const string CONNECTION_STRING_ENV_VAR = "ConnectionStrings__DefaultConnection";

    public ApplicationContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable(CONNECTION_STRING_ENV_VAR) ?? DEFAULT_CONNECTION_STRING;

        DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationContext(optionsBuilder.Options);
    }
}
