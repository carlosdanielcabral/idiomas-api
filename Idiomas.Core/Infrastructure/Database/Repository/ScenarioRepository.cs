using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class ScenarioRepository(ApplicationContext database) : IScenarioRepository
{
    private readonly ApplicationContext _database = database;

    public async Task<IEnumerable<Scenario>> GetByLanguage(Language? language)
    {
        IQueryable<ScenarioModel> query = this._database.Scenario.Where(s => s.IsActive);

        if (language.HasValue)
        {
            query = query.Where(s => s.Language == language.Value.ToString());
        }

        List<ScenarioModel> models = await query.ToListAsync();

        return models.ToEntities();
    }

    public async Task<Scenario?> GetById(string id)
    {
        if (!Guid.TryParse(id, out var scenarioId))
            return null;

        ScenarioModel? model = await this._database.Scenario
            .FirstOrDefaultAsync(s => s.Id == scenarioId);

        return model?.ToEntity();
    }
}
