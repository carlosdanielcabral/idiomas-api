using IdiomasAPI.Source.Interface.Controller;

namespace IdiomasAPI.Source.Presentation.Http.Controller;

public static class DependencyInjection
{
    public static IServiceCollection AddControllers(this IServiceCollection services)
    {
        services.AddScoped<IUserController, UserController>();

        return services;
    }
}