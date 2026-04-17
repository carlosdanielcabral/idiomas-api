using System.Security.Claims;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Presentation.DTO.Conversation;

namespace Idiomas.Core.Interface.Controller;

public interface IConversationController
{
    Task<IResult> StartConversation(CreateConversationRequestDTO request, ClaimsPrincipal user, StartConversation useCase);
    Task<IResult> SendMessage(string conversationId, SendMessageRequestDTO request, ClaimsPrincipal user, SendMessage useCase);
    Task<IResult> ListScenarios(Language language, ListScenarios useCase);
    Task<IResult> GetConversation(string conversationId, ClaimsPrincipal user, GetConversation useCase);
    Task<IResult> ListConversations(ClaimsPrincipal user, ListConversations useCase);
    Task<IResult> EndConversation(string conversationId, ClaimsPrincipal user, EndConversation useCase);
}
