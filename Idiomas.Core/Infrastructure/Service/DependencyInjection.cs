
using System.Text;
using Idiomas.Source.Infrastructure.Service.Authentication;
using Idiomas.Source.Infrastructure.Service.Hash;
using Idiomas.Source.Interface.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Idiomas.Source.Infrastructure.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IHash, Argon2Hash>();
        services.AddInfraAuthentication(configuration);

        return services;
    }

    public static IServiceCollection AddInfraAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IToken, JWT>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured."))),

                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],

                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromSeconds(30) 
            };
        });

        return services;
    }
}