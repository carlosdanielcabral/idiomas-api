using Idiomas.Core.Infrastructure.Service.Hash;

namespace Idiomas.Tests.Core.Infrastructure.Service.Hash;

public class Sha256TokenHasherTest
{
    private readonly Sha256TokenHasher _hasher = new();

    [Fact]
    public void Hash_ReturnsDeterministicHashForSameInput()
    {
        const string TOKEN = "my-secret-token-123";

        string hash1 = this._hasher.Hash(TOKEN);
        string hash2 = this._hasher.Hash(TOKEN);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Hash_ReturnsDifferentHashForDifferentInput()
    {
        string hash1 = this._hasher.Hash("token-a");
        string hash2 = this._hasher.Hash("token-b");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Hash_ReturnsNonEmptyHash()
    {
        string hash = this._hasher.Hash("any-token");

        Assert.False(string.IsNullOrEmpty(hash));
    }

    [Fact]
    public void Hash_ReturnsHexadecimalString()
    {
        string hash = this._hasher.Hash("any-token");

        Assert.True(hash.All(character => "0123456789abcdef".Contains(character)));
    }
}
