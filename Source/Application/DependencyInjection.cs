using IdiomasAPI.Source.Application.UseCase.AuthCase;
using IdiomasAPI.Source.Application.UseCase.UserCase;

namespace IdiomasAPI.Source.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUser>();
        services.AddScoped<MailPasswordLogin>();

        return services;
    }
}