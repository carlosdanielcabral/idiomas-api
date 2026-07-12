namespace Idiomas.Core.Interface.Service;

public interface ITokenGenerator
{
    TokenPair Generate();
}

public record TokenPair(string RawToken, string TokenHash);
