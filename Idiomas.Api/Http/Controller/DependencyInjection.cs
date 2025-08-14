using System.Text.Json;
using Idiomas.Api.Http.Controller;
using Idiomas.Api.Interface.Controller;

namespace Idiomas.Api.Http.Controller;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationControllers(this IServiceCollection services)
    {
        services.AddScoped<IUserController, UserController>();
        services.AddScoped<IAuthController, AuthController>();
        services.AddScoped<IDictionaryController, DictionaryController>();
        services.AddScoped<IFileController, FileController>();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return services;
    }
}