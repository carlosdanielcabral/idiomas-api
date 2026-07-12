namespace Idiomas.Core.Domain.Entity;

public class EmailVerificationToken(
    Guid id,
    Guid userId,
    string tokenHash,
    DateTime createdAt,
    DateTime expiresAt,
    DateTime? usedAt = null)
{
    private const int TOKEN_EXPIRATION_HOURS = 1;

    public Guid Id { get; private set; } = id;
    public Guid UserId { get; private set; } = userId;
    public string TokenHash { get; private set; } = tokenHash;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime ExpiresAt { get; private set; } = expiresAt;
    public DateTime? UsedAt { get; private set; } = usedAt;

    public bool IsExpired => DateTime.UtcNow > this.ExpiresAt;

    public bool IsUsed => this.UsedAt != null;

    public bool IsValid => !this.IsExpired && !this.IsUsed;

    public static EmailVerificationToken Create(Guid userId, string tokenHash)
    {
        DateTime now = DateTime.UtcNow;

        return new EmailVerificationToken(
            Guid.NewGuid(),
            userId,
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
