using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Interface.Repository;

public interface IScenarioRepository
{
    Task<IEnumerable<Scenario>> GetByLanguage(Language? language);
    Task<Scenario?> GetById(string id);
}
