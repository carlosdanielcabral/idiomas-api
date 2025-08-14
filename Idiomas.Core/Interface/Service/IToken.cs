using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Interface.Service;

public interface IToken
{
    public string Generate(User user);
}