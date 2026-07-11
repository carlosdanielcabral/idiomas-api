namespace Idiomas.Core.Interface.Service;

public interface ITokenHasher
{
    string Hash(string token);
}
