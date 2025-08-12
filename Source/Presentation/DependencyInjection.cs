using IdiomasAPI.Source.Presentation.Http;
namespace IdiomasAPI.Source.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHttp();

        return services;
    }
}