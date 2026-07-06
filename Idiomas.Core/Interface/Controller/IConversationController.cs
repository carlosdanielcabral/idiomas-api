using System.Security.Claims;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Presentation.DTO.Conversation;
using Idiomas.Core.Presentation.Http.Validator.Conversation;

namespace Idiomas.Core.Interface.Controller;

public interface IConversationController
{
    Task<IResult> StartConversation(CreateConversationRequestDTO request, ClaimsPrincipal user, StartConversationValidator validator, StartConversation useCase);
    Task<IResult> SendMessage(string conversationId, SendMessageRequestDTO request, ClaimsPrincipal user, SendMessageValidator validator, SendMessage useCase);
    Task<IResult> ListScenarios(string? language, ListScenarios useCase);
    Task<IResult> GetConversation(string conversationId, ClaimsPrincipal user, GetConversation useCase);
    Task<IResult> ListConversations(ClaimsPrincipal user, ListConversations useCase);
    Task<IResult> EndConversation(string conversationId, ClaimsPrincipal user, EndConversation useCase);
}
