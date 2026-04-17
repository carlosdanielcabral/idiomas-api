using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Domain.Entity;

public class Message
{
    public string Id { get; private set; }
    public string ConversationId { get; private set; }
    public MessageRole Role { get; private set; }
    public string Content { get; private set; }
    public ICollection<Correction> Corrections { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Message(string id, string conversationId, MessageRole role, string content)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(conversationId))
            throw new ArgumentException("ConversationId is required.", nameof(conversationId));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        Id = id;
        ConversationId = conversationId;
        Role = role;
        Content = content;
        Corrections = new List<Correction>();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddCorrection(Correction correction)
    {
        Corrections.Add(correction);
    }
}
