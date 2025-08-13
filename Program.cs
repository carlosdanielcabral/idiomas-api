using IdiomasAPI.Source.Application;
using IdiomasAPI.Source.Helper;
using IdiomasAPI.Source.Infrastructure;
using IdiomasAPI.Source.Presentation;
using IdiomasAPI.Source.Presentation.Http;
using IdiomasAPI.Source.Presentation.Http.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHelpers()
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddApplication()
    .AddPresentation()
    .AddAuthorization();

WebApplication app = builder.Build();

app.AddAPIDocumentation();
app.AddMiddlewares();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var router = scope.ServiceProvider.GetRequiredService<Router>();
    router.Register(app);
}

var apiUrl = Environment.GetEnvironmentVariable("API_URL");

if (!string.IsNullOrEmpty(apiUrl))
{
    app.Run(apiUrl);
}
else
{
    app.Run();
}

