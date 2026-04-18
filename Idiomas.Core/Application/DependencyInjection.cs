using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Application.UseCase.DictionaryCase;
using Idiomas.Core.Application.UseCase.File;
using Idiomas.Core.Application.UseCase.UserCase;

namespace Idiomas.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // User
        services.AddScoped<CreateUser>();
        services.AddScoped<UpdateUser>();

        // Auth
        services.AddScoped<MailPasswordLogin>();

        // Dictionary
        services.AddScoped<CreateWord>();
        services.AddScoped<ListWords>();
        services.AddScoped<UpdateWord>();
        services.AddScoped<DeleteWord>();

        // File
        services.AddScoped<RequestFileUpload>();
        services.AddScoped<ConfirmFileUpload>();
        services.AddScoped<FailFileUpload>();

        // Conversation
        services.AddScoped<StartConversation>();
        services.AddScoped<SendMessage>();
        services.AddScoped<ListScenarios>();
        services.AddScoped<GetConversation>();
        services.AddScoped<ListConversations>();
        services.AddScoped<EndConversation>();

        return services;
    }
}