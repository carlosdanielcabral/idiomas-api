namespace Idiomas.Source.Helper;

public static class DependencyInjection
{
    public static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        services.AddScoped<FileHelper>();

        return services;
    }
}