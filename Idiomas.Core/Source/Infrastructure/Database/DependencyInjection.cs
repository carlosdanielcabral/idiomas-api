
using Idiomas.Source.Infrastructure.Database.Context;
using Idiomas.Source.Infrastructure.Database.Repository;
using Idiomas.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Source.Infrastructure.Database;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string for Database is not configured");

        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDictionaryRepository, DictionaryRepository>();
        services.AddScoped<IFileRepository, FileRepository>();

        return services;
    }
}