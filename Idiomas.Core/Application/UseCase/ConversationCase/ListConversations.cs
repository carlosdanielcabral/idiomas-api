using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class ListConversations(IConversationRepository conversationRepository)
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;

    public async Task<IEnumerable<Conversation>> Execute(string userId)
    {
        return await this._conversationRepository.GetByUserId(userId);
    }
}
