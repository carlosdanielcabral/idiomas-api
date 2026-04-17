using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using System.Net;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class EndConversation(IConversationRepository conversationRepository)
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;

    public async Task Execute(string conversationId, string userId)
    {
        Conversation? conversation = await this._conversationRepository.GetById(conversationId);

        if (conversation == null)
        {
            throw new ApiException(
                $"Conversation with ID {conversationId} not found.",
                HttpStatusCode.NotFound);
        }

        if (conversation.UserId != userId)
        {
            throw new ApiException(
                "You do not have permission to access this conversation.",
                HttpStatusCode.Forbidden);
        }

        await this._conversationRepository.Inactivate(conversationId);
    }
}
