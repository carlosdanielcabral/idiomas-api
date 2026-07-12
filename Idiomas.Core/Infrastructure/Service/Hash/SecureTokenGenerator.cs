using System.Security.Cryptography;
using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Infrastructure.Service.Hash;

public class SecureTokenGenerator(ITokenHasher tokenHasher) : ITokenGenerator
{
    private const int TOKEN_LENGTH = 64;

    private readonly ITokenHasher _tokenHasher = tokenHasher;

    public TokenPair Generate()
    {
        string rawToken = RandomNumberGenerator.GetHexString(TOKEN_LENGTH);

        string tokenHash = this._tokenHasher.Hash(rawToken);

        return new TokenPair(rawToken, tokenHash);
    }
}
