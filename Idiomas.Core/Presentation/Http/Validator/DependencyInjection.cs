using Idiomas.Core.Presentation.Http.Validator.Auth;
using Idiomas.Core.Presentation.Http.Validator.Conversation;
using Idiomas.Core.Presentation.Http.Validator.Dictionary;
using Idiomas.Core.Presentation.Http.Validator.File;
using Idiomas.Core.Presentation.Http.Validator.User;

namespace Idiomas.Core.Presentation.Http.Validator;

public static class DependencyInjection
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<CreateUserValidator>();
        services.AddScoped<UpdateUserValidator>();
        services.AddScoped<MailPasswordLoginValidator>();
        services.AddScoped<GoogleLoginValidator>();
        services.AddScoped<ForgotPasswordValidator>();
        services.AddScoped<ResetPasswordValidator>();
        services.AddScoped<ResendVerificationValidator>();
        services.AddScoped<CreateWordValidator>();
        services.AddScoped<UpdateWordValidator>();
        services.AddScoped<StartConversationValidator>();
        services.AddScoped<SendMessageValidator>();
        services.AddScoped<RequestFileUploadValidator>();

        return services;
    }
}
