namespace IdiomasAPI.Source.Presentation.Http.Route;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationRoutes(this IServiceCollection services)
    {
        services.AddScoped<UserRoute>();
        services.AddScoped<AuthRoute>();

        return services;
    }
}