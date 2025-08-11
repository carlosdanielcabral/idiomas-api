using IdiomasAPI.Source.Application;
using IdiomasAPI.Source.Infrastructure;
using IdiomasAPI.Source.Presentation;
using IdiomasAPI.Source.Presentation.Http;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddPresentation();

WebApplication app = builder.Build();

var router = app.Services.GetRequiredService<Router>();

router.Register(app);

var apiUrl = Environment.GetEnvironmentVariable("API_URL");

if (!string.IsNullOrEmpty(apiUrl))
{
    app.Run(apiUrl);
}
else
{
    app.Run();
}

