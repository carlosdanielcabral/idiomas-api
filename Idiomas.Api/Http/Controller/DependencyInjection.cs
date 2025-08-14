using System.Text.Json;
using Idiomas.Core.Application.Http.Controller;
using Idiomas.Core.Interface.Controller;

namespace Idiomas.Core.Presentation.Http.Controller;

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