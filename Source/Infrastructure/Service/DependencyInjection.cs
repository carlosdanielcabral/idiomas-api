
using IdiomasAPI.Source.Infrastructure.Service.Hash;
using IdiomasAPI.Source.Interface.Service;

namespace IdiomasAPI.Source.Infrastructure.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IHash, Argon2Hash>();

        return services;
    }
}