using System.Net;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Domain.Enum.Extensions;
using Idiomas.Core.Interface.Repository;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class ListScenarios(IScenarioRepository scenarioRepository)
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository;

    public async Task<IEnumerable<Scenario>> Execute(string? language)
    {
        Language? parsedLanguage = this.ParseLanguage(language);

        return await this._scenarioRepository.GetByLanguage(parsedLanguage);
    }

    private Language? ParseLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
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
