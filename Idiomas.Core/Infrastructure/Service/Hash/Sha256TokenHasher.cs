using System.Security.Cryptography;
using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Infrastructure.Service.Hash;

public class Sha256TokenHasher : ITokenHasher
{
    public string Hash(string token)
    {
        byte[] bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
