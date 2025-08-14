using Idiomas.Source.Domain.Entity;

namespace Idiomas.Source.Interface.Service;

public interface IToken
{
    public string Generate(User user);
}