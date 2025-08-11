using IdiomasAPI.Source.Presentation.Http;
using IdiomasAPI.Source.Presentation.Http.Route;

namespace IdiomasAPI.Source.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddRoutes();
        services.AddScoped<Router>();

        return services;
    }
}