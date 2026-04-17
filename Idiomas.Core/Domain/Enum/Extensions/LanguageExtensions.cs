namespace Idiomas.Core.Domain.Enum.Extensions;

public static class LanguageExtensions
{
    public static string GetAvailableLanguagesString()
    {
        string[] availableLanguages = Enum.GetNames<Language>();

        return string.Join(", ", availableLanguages);
    }
}
