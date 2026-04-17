using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Model;

namespace Idiomas.Core.Infrastructure.Database.Mapper;

public static class ScenarioModelMapper
{
    public static Scenario ToEntity(this ScenarioModel model)
    {
        if (!Enum.TryParse<Language>(model.Language, out var language))
        {
            throw new InvalidOperationException($"Invalid language value in database: {model.Language}");
        }

        Scenario scenario = new(
            model.Id.ToString(),
            language,
            model.Title,
            model.Description
        );

        return scenario;
    }

    public static ScenarioModel ToModel(this Scenario entity)
    {
        ScenarioModel model = new()
        {
            Id = Guid.Parse(entity.Id),
            Language = entity.Language.ToString(),
            Title = entity.Title,
            Description = entity.Description,
            IsActive = entity.IsActive
        };

        return model;
    }

    public static IEnumerable<Scenario> ToEntities(this IEnumerable<ScenarioModel> models)
    {
        return models.Select(m => m.ToEntity());
    }
}
