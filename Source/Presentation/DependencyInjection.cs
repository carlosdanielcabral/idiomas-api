using Idiomas.Source.Presentation.Http;
namespace Idiomas.Source.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHttp();

        return services;
    }
}