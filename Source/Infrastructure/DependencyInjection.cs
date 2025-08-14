using Idiomas.Source.Infrastructure.Database;
using Idiomas.Source.Infrastructure.Service;
using Idiomas.Source.Infrastructure.Storage;

namespace Idiomas.Source.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDatabase(configuration);
        services.AddServices(configuration);
        services.AddStorage(configuration, environment);

        return services;
    }
}