using Idiomas.Core.Infrastructure.Database;
using Idiomas.Core.Infrastructure.Service;
using Idiomas.Core.Infrastructure.Storage;

namespace Idiomas.Core.Infrastructure;

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