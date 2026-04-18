using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Interface.Service;

public interface IConversationLLMService
{
    Task<ConversationLLMResponse> SendMessageAsync(
        Conversation conversation,
        string userMessage,
        string? scenarioDescription);
}
