using IdiomasAPI.Source.Presentation.Http.Route;
using IdiomasAPI.Source.Presentation.Http.Controller;
using IdiomasAPI.Source.Presentation.Http;

namespace IdiomasAPI.Source.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddPresentationControllers()
            .AddPresentationRoutes()
            .AddScoped<Router>();

        return services;
    }
}