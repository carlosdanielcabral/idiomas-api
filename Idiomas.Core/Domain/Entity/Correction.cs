using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Domain.Entity;

public class Correction
{
    public string Id { get; private set; }
    public string MessageId { get; private set; }
    public string OriginalFragment { get; private set; }
    public string SuggestedFragment { get; private set; }
    public string Explanation { get; private set; }
    public ErrorType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Correction(
        string id,
        string messageId,
        string originalFragment,
        string suggestedFragment,
        string explanation,
        ErrorType type)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(messageId))
            throw new ArgumentException("MessageId is required.", nameof(messageId));
        if (string.IsNullOrWhiteSpace(originalFragment))
            throw new ArgumentException("OriginalFragment is required.", nameof(originalFragment));
        if (string.IsNullOrWhiteSpace(suggestedFragment))
            throw new ArgumentException("SuggestedFragment is required.", nameof(suggestedFragment));
        if (string.IsNullOrWhiteSpace(explanation))
            throw new ArgumentException("Explanation is required.", nameof(explanation));

        Id = id;
        MessageId = messageId;
        OriginalFragment = originalFragment;
        SuggestedFragment = suggestedFragment;
        Explanation = explanation;
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }
}
