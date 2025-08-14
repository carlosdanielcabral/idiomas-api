using Idiomas.Source.Application.UseCase.AuthCase;
using Idiomas.Source.Application.UseCase.DictionaryCase;
using Idiomas.Source.Application.UseCase.File;
using Idiomas.Source.Application.UseCase.UserCase;

namespace Idiomas.Source.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUser>();
        services.AddScoped<MailPasswordLogin>();
        services.AddScoped<CreateWord>();
        services.AddScoped<ListWords>();
        services.AddScoped<UpdateWord>();
        services.AddScoped<DeleteWord>();
        services.AddScoped<RequestFileUpload>();
        services.AddScoped<ConfirmFileUpload>();
        services.AddScoped<FailFileUpload>();

        return services;
    }
}