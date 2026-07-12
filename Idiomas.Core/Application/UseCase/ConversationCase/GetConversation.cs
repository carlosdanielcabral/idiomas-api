using Idiomas.Core.Application.Exceptions.Conversation;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class GetConversation(IConversationRepository conversationRepository)
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;

    public async Task<Conversation> Execute(string conversationId, string userId)
    {
        Conversation? conversation = await this._conversationRepository.GetById(conversationId);

        if (conversation == null)
        {
            throw new ConversationNotFoundException();
        }

        if (conversation.UserId != userId)
        {
            throw new ConversationAccessDeniedException();
        }

        return conversation;
    }
}
