using IdiomasAPI.Source.Application.UseCase;

namespace IdiomasAPI.Source.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUser>();

        return services;
    }
}