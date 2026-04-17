using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Route;
using Idiomas.Core.Presentation.DTO.Conversation;

namespace Idiomas.Core.Presentation.Http.Route;

public class ConversationRoute(IConversationController controller) : IRoute
{
    private readonly IConversationController _controller = controller;

    public void Register(WebApplication app)
    {
        var conversations = app.MapGroup("/conversations").RequireAuthorization();

        // List scenarios
        conversations.MapGet("/scenarios", (Language? language, IServiceProvider provider) =>
        {
            var useCase = provider.GetRequiredService<Application.UseCase.ConversationCase.ListScenarios>();
            return this._controller.ListScenarios(language, useCase);
        })
        .Produces<List<ScenarioResponseDTO>>(StatusCodes.Status200OK);

        // Start conversation
        conversations.MapPost("/", (CreateConversationRequestDTO dto, HttpContext context, IServiceProvider provider) =>
        {
            var useCase = provider.GetRequiredService<Application.UseCase.ConversationCase.StartConversation>();
            return this._controller.StartConversation(dto, context.User, useCase);
        })
        .Produces<ConversationResponseDTO>(StatusCodes.Status201Created);

        // Send message
        conversations.MapPost("/{id}/messages", (string id, SendMessageRequestDTO dto, HttpContext context, IServiceProvider provider) =>
        {
            var useCase = provider.GetRequiredService<Application.UseCase.ConversationCase.SendMessage>();
            return this._controller.SendMessage(id, dto, context.User, useCase);
        })
        .Produces<MessageResponseDTO>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // Get conversation
        conversations.MapGet("/{id}", (string id, HttpContext context, IServiceProvider provider) =>
        {
            var useCase = provider.GetRequiredService<Application.UseCase.ConversationCase.GetConversation>();
            return this._controller.GetConversation(id, context.User, useCase);
        })
        .Produces<ConversationDetailResponseDTO>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // List conversations
        conversations.MapGet("/", (HttpContext context, IServiceProvider provider) =>
        {
            var useCase = provider.GetRequiredService<Application.UseCase.ConversationCase.ListConversations>();
            return this._controller.ListConversations(context.User, useCase);
        })
        .Produces<ConversationListWrapperDTO>(StatusCodes.Status200OK);

        // End conversation
        conversations.MapPatch("/{id}/end", (string id, HttpContext context, IServiceProvider provider) =>
        {
            var useCase = provider.GetRequiredService<Application.UseCase.ConversationCase.EndConversation>();
            return this._controller.EndConversation(id, context.User, useCase);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
