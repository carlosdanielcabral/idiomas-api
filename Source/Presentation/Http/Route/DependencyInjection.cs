using IdiomasAPI.Source.Interface.Route;
using IdiomasAPI.Source.Presentation.Http.Route;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public static class DependencyInjection
{
    public static IServiceCollection AddRoutes(this IServiceCollection services)
    {
        services.AddScoped<IUserRoute, UserRoute>();

        return services;
    }
}