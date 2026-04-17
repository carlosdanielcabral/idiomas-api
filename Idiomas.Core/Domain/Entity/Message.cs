using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Domain.Entity;

public class Message(
    string id,
    string conversationId,
    MessageRole role,
    string content)
{
    public string Id { get; private set; } = id;
    public string ConversationId { get; private set; } = conversationId;
    public MessageRole Role { get; private set; } = role;
    public string Content { get; private set; } = content;
    public ICollection<Correction> Corrections { get; private set; } = new List<Correction>();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void AddCorrection(Correction correction)
    {
        Corrections.Add(correction);
    }
}
