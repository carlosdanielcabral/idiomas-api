using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;

namespace Idiomas.Core.Application.UseCase.ConversationCase;

public class ListScenarios(IScenarioRepository scenarioRepository)
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository;

    public async Task<IEnumerable<Scenario>> Execute(Language language)
    {
        return await this._scenarioRepository.GetByLanguage(language);
    }
}
