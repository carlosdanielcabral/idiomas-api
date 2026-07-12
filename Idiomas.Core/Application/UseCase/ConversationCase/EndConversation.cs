using Idiomas.Core.Application.Error.Conversation;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class EndConversation(IConversationRepository conversationRepository)
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;

    public async Task Execute(string conversationId, string userId)
    {
        await this.ValidateConversation(conversationId, userId);

        await this._conversationRepository.Inactivate(conversationId);
    }

    private async Task ValidateConversation(string conversationId, string userId)
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
    }
}
