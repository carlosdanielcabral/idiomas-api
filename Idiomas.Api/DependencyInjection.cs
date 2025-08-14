using Idiomas.Api.Http;
namespace Idiomas.Api.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHttp();

        return services;
    }
}