namespace Idiomas.Core.Domain.Entity;

public class PasswordResetToken(Guid id, Guid userId, string token, DateTime createdAt, DateTime expiresAt, DateTime? usedAt = null)
{
    public Guid Id { get; private set; } = id;
    public Guid UserId { get; private set; } = userId;
    public string Token { get; private set; } = token;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime ExpiresAt { get; private set; } = expiresAt;
    public DateTime? UsedAt { get; set; } = usedAt;

    public bool IsExpired => DateTime.UtcNow > this.ExpiresAt;

    public bool IsUsed => this.UsedAt != null;
}
