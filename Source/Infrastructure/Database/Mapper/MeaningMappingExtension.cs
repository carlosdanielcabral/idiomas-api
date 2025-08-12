using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Model;

namespace IdiomasAPI.Source.Infrastructure.Database.Mapper;

public static class MeaningMappingExtension
{
    public static Meaning ToEntity(this MeaningModel model)
    {
        return new Meaning(model.Id, model.Meaning, model.Example);
    }

    public static MeaningModel ToModel(this Meaning entity)
    {
        return new MeaningModel() { Id = entity.Id, Meaning = entity.Definition, Example = entity.Example };
    }

    public static ICollection<Meaning> ToEntities(this IEnumerable<MeaningModel> models)
    {
        return [.. models.Select(model => model.ToEntity())];
    }

    public static ICollection<MeaningModel> ToModels(this IEnumerable<Meaning> entities)
    {
        return [.. entities.Select(entity => entity.ToModel())];
    }
}