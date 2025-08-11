using IdiomasAPI.Source.Interface.Service;
using Isopoh.Cryptography.Argon2;

namespace IdiomasAPI.Source.Infrastructure.Service.Hash;

public class Argon2Hash : IHash
{
    public string Hash(string text)
    {
        return Argon2.Hash(text);
    }

    public bool Verify(string text, string hash)
    {
        return  Argon2.Verify(hash, text);
    }
}