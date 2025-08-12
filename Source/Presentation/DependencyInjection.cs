using IdiomasAPI.Source.Presentation.Http.Route;
using IdiomasAPI.Source.Presentation.Http.Controller;
using IdiomasAPI.Source.Presentation.Http;
using Microsoft.OpenApi.Models;

namespace IdiomasAPI.Source.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddPresentationControllers()
            .AddPresentationRoutes()
            .AddScoped<Router>();

        return services;
    }

    public static WebApplication AddAPIDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        return app;
    }

    public static IServiceCollection AddAPIDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Idiomas API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Carlos Daniel Cabral",
                    Email = "dev.carlosdaniel@gmail.com"
                }
            });
        });

        return services;
    }
}