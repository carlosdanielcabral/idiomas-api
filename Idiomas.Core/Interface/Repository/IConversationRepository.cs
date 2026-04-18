using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Interface.Repository;

public interface IConversationRepository
{
    Task<Conversation> Insert(Conversation conversation);
    Task<Conversation?> GetById(string id);
    Task<IEnumerable<Conversation>> GetByUserId(string userId);
    Task Inactivate(string id);
    Task<Message> InsertMessage(Message message);
    Task<Correction> InsertCorrection(Correction correction);
}
