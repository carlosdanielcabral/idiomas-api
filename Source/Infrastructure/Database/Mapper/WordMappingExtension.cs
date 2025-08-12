using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Model;

namespace IdiomasAPI.Source.Infrastructure.Database.Mapper;

public static class WordMappingExtension
{
    public static Word ToEntity(this WordModel model)
    {
        return new Word(model.Id.ToString(), model.Word, model.Ipa, model.UserId.ToString(), model.Meanings.ToEntities());
    }

    public static WordModel ToModel(this Word entity)
    {
        return new WordModel() { Id = Guid.Parse(entity.Id), Word = entity.Name, Ipa = entity.Ipa, UserId = Guid.Parse(entity.UserId), Meanings = entity.Meanings.ToModels() };
    }

    public static IEnumerable<Word> ToEntities(this IEnumerable<WordModel> models)
    {
        return models.Select(model => model.ToEntity());
    }
}