using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Domain.Entity;

public class Conversation(
    string id,
    string userId,
    Language language,
    ConversationMode mode,
    string? scenarioId = null,
    bool isActive = true)
{
    public string Id { get; private set; } = id;
    public string UserId { get; private set; } = userId;
    public Language Language { get; private set; } = language;
    public ConversationMode Mode { get; private set; } = mode;
    public string? ScenarioId { get; private set; } = scenarioId;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public ICollection<Message> Messages { get; private set; } = new List<Message>();
    public bool IsActive { get; private set; } = isActive;

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
