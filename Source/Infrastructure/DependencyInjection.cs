using IdiomasAPI.Source.Infrastructure.Database.Repository;
using IdiomasAPI.Source.Infrastructure.Service;
using IdiomasAPI.Source.Infrastructure.Storage;

namespace IdiomasAPI.Source.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddServices(configuration);
        services.AddStorage();

        return services;
    }
}