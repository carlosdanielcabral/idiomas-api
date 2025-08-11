using IdiomasAPI.Source.Interface.Route;

namespace IdiomasAPI.Source.Presentation.Http.Route;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationRoutes(this IServiceCollection services)
    {
        services.AddScoped<IUserRoute, UserRoute>();

        return services;
    }
}