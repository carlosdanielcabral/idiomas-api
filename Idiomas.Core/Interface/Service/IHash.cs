namespace Idiomas.Core.Interface.Service;

public interface IHash
{
    public string Hash(string text);
    public bool Verify(string text, string hash);
}