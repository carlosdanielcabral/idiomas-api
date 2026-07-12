using Idiomas.Core.Helper.Error;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Domain.Enum.Extensions;

namespace Idiomas.Core.Helper;

public static class LanguageHelper
{
    public static Language? ParseLanguage(string? language, bool isRequired = false)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            if (isRequired)
            {
                throw new LanguageRequiredException();
            }

            return null;
        }

        bool isValidLanguage = Enum.TryParse<Language>(language, ignoreCase: true, out Language parsedLanguage);

        if (!isValidLanguage)
        {
            string availableLanguagesString = LanguageExtensions.GetAvailableLanguagesString();

            throw new LanguageInvalidException(language, availableLanguagesString);
        }

        return parsedLanguage;
    }
}
