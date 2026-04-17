using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Domain.Entity;

public class Conversation
{
    public string Id { get; private set; }
    public string UserId { get; private set; }
    public Language Language { get; private set; }
    public ConversationMode Mode { get; private set; }
    public string? ScenarioId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ICollection<Message> Messages { get; private set; }
    public bool IsActive { get; private set; }

    public Conversation(string id, string userId, Language language, ConversationMode mode)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        Id = id;
        UserId = userId;
        Language = language;
        Mode = mode;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Messages = new List<Message>();
        IsActive = true;
    }

    public void SetScenarioId(string scenarioId)
    {
        ScenarioId = scenarioId;
    }

    public void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMessage(Message message)
    {
        Messages.Add(message);
        this.Touch();
    }
}
