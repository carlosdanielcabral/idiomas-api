namespace Idiomas.Source.Presentation.Http.Middleware;

public static class DependencyInjection
{
    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ApiExceptionMiddleware>();

        return app;
    }
}