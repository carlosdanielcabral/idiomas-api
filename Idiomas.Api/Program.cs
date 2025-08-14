using Idiomas.Core.Application;
using Idiomas.Core.Helper;
using Idiomas.Core.Infrastructure;
using Idiomas.Api;
using Idiomas.Api.Http;
using Idiomas.Api.Http.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHelpers()
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddApplication()
    .AddPresentation()
    .AddAuthorization();

WebApplication app = builder.Build();

app.AddAPIDocumentation()
    .AddMiddlewares()
    .UseAuthentication()
    .UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    Router router = scope.ServiceProvider.GetRequiredService<Router>();
    router.Register(app);
}

string apiUrl = Environment.GetEnvironmentVariable("API_URL")
    ?? throw new InvalidOperationException("API url is not configured");


app.Run(apiUrl);
