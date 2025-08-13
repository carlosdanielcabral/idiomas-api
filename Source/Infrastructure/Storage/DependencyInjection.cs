using IdiomasAPI.Source.Infrastructure.Storage.Adapter;
using IdiomasAPI.Source.Interface.Storage;

namespace IdiomasAPI.Source.Infrastructure.Storage;

public static class DependencyInjection
{
    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddScoped<IFileStorage, AzureBlobStorage>();

        return services;
    }
}