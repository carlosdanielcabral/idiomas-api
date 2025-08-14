using Idiomas.Core.Presentation.Http;
namespace Idiomas.Core.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHttp();

        return services;
    }
}