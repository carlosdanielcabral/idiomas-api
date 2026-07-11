using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;

namespace Idiomas.Tests.Core.Domain.Entity;

public class UserCredentialTest
{
    [Fact]
    public void UpdatePasswordHash_SetsNewHash()
    {
        UserCredential credential = new(
            "credential-id",
            "user-id",
            AuthProvider.Local,
            "old-hash",
            null
        );

        credential.UpdatePasswordHash("new-hash");

        Assert.Equal("new-hash", credential.PasswordHash);
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        UserCredential credential = new(
            "credential-id",
            "user-id",
            AuthProvider.Google,
            null,
            "google-sub-123"
        );

        Assert.Equal("credential-id", credential.Id);
        Assert.Equal("user-id", credential.UserId);
        Assert.Equal(AuthProvider.Google, credential.Provider);
        Assert.Null(credential.PasswordHash);
        Assert.Equal("google-sub-123", credential.ExternalSubject);
    }
}
