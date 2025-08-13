using System.Text.Json;
using IdiomasAPI.Source.Interface.Controller;

namespace IdiomasAPI.Source.Presentation.Http.Controller;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationControllers(this IServiceCollection services)
    {
        services.AddScoped<IUserController, UserController>();
        services.AddScoped<IAuthController, AuthController>();
        services.AddScoped<IDictionaryController, DictionaryController>();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return services;
    }
}