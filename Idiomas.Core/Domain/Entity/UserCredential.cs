using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;

namespace Idiomas.Core.Domain.Entity;

public class UserCredential(
    string id,
    string userId,
    AuthProvider provider,
    string? passwordHash,
    string? externalSubject)
{
    public string Id { get; private set; } = id;
    public string UserId { get; private set; } = userId;
    public AuthProvider Provider { get; private set; } = provider;
    public string? PasswordHash { get; private set; } = passwordHash;
    public string? ExternalSubject { get; private set; } = externalSubject;

    public static UserCredential Create(string userId, AuthProvider provider, string? passwordHash, string? externalSubject = null)
    {
        return new UserCredential(
            UUIDGenerator.Generate(),
            userId,
            provider,
            passwordHash,
            externalSubject
        );
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        this.PasswordHash = passwordHash;
    }
}
