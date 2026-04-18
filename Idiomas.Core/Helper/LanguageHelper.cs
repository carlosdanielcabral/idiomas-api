using System.Net;
using Idiomas.Core.Application.Error;
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
                throw new ApiException("Language is required.", HttpStatusCode.BadRequest);
            }

            return null;
        }

        bool isValidLanguage = Enum.TryParse<Language>(language, ignoreCase: true, out Language parsedLanguage);

        if (!isValidLanguage)
        {
            string availableLanguagesString = LanguageExtensions.GetAvailableLanguagesString();

            throw new ApiException(
                $"Invalid language '{language}'. Available languages: {availableLanguagesString}",
                HttpStatusCode.BadRequest);
        }

        return parsedLanguage;
    }
}
