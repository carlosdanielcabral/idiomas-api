using IdiomasAPI.Source.Infrastructure;
using IdiomasAPI.Source.Presentation;
using IdiomasAPI.Source.Presentation.Http;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

WebApplication app = builder.Build();

var router = app.Services.GetRequiredService<Router>();

router.Register(app);

app.Run();

