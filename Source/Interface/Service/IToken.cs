using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Interface.Service;

public interface IToken
{
    public string Generate(User user);
}