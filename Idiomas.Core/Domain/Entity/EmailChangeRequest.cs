namespace Idiomas.Core.Domain.Entity;

public class EmailChangeRequest(
    Guid id,
    Guid userId,
    string newEmail,
    string tokenHash,
    DateTime createdAt,
    DateTime expiresAt,
    DateTime? usedAt = null)
{
    private const int TOKEN_EXPIRATION_HOURS = 1;

    public Guid Id { get; private set; } = id;
    public Guid UserId { get; private set; } = userId;
    public string NewEmail { get; private set; } = newEmail;
    public string TokenHash { get; private set; } = tokenHash;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime ExpiresAt { get; private set; } = expiresAt;
    public DateTime? UsedAt { get; private set; } = usedAt;

    public bool IsExpired => DateTime.UtcNow > this.ExpiresAt;

    public bool IsUsed => this.UsedAt != null;

    public bool IsValid => !this.IsExpired && !this.IsUsed;

    public static EmailChangeRequest Create(Guid userId, string newEmail, string tokenHash)
    {
        DateTime now = DateTime.UtcNow;

        return new EmailChangeRequest(
            Guid.NewGuid(),
            userId,
            newEmail,
            tokenHash,
            now,
            now.AddHours(TOKEN_EXPIRATION_HOURS)
        );
    }

    public void MarkAsUsed()
    {
        this.UsedAt = DateTime.UtcNow;
    }
}
