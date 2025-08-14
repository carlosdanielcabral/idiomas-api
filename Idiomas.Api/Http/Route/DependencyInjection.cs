namespace Idiomas.Api.Http.Route;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationRoutes(this IServiceCollection services)
    {
        services.AddScoped<UserRoute>();
        services.AddScoped<AuthRoute>();
        services.AddScoped<DictionaryRoute>();
        services.AddScoped<FileRoute>();

        return services;
    }
}