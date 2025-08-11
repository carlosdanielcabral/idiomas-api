using IdiomasAPI.Source.Infrastructure.Database.Repository;

namespace IdiomasAPI.Source.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        return services;
    }
}