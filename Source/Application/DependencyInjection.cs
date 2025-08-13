using IdiomasAPI.Source.Application.UseCase.AuthCase;
using IdiomasAPI.Source.Application.UseCase.DictionaryCase;
using IdiomasAPI.Source.Application.UseCase.UserCase;

namespace IdiomasAPI.Source.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUser>();
        services.AddScoped<MailPasswordLogin>();
        services.AddScoped<CreateWord>();
        services.AddScoped<ListWords>();

        return services;
    }
}