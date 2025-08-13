
using IdiomasAPI.Source.Infrastructure.Database.Context;
using IdiomasAPI.Source.Infrastructure.Database.Repository;
using IdiomasAPI.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace IdiomasAPI.Source.Infrastructure.Database;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDictionaryRepository, DictionaryRepository>();

        return services;
    }
}