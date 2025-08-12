using IdiomasAPI.Source.Presentation.Http.Route;
using IdiomasAPI.Source.Presentation.Http.Controller;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;

namespace IdiomasAPI.Source.Presentation.Http;

public static class DependencyInjection
{
    public static IServiceCollection AddHttp(this IServiceCollection services)
    {
        services
            .AddRateLimit()
            .AddPresentationControllers()
            .AddPresentationRoutes()
            .AddAPIDocumentation()
            .AddScoped<Router>();

        return services;
    }
    
    public static IServiceCollection AddRateLimit(this IServiceCollection services)
    {
        services.AddRateLimiter(static options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(static httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: static partition => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

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
        services.AddSwaggerGen(static options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Idiomas API",
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