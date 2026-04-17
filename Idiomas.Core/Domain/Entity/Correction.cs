using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Domain.Entity;

public class Correction(
    string id,
    string messageId,
    string originalFragment,
    string suggestedFragment,
    string explanation,
    ErrorType type)
{
    public string Id { get; private set; } = id;
    public string MessageId { get; private set; } = messageId;
    public string OriginalFragment { get; private set; } = originalFragment;
    public string SuggestedFragment { get; private set; } = suggestedFragment;
    public string Explanation { get; private set; } = explanation;
    public ErrorType Type { get; private set; } = type;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
