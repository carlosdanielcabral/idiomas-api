using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Infrastructure.Database.Model;

namespace Idiomas.Source.Infrastructure.Database.Mapper;

public static class CFileMappingExtension
{
    public static CFile ToEntity(this FileModel model)
    {
        return new CFile(model.Id.ToString(), model.OriginalName, model.Key, model.MimeType, model.Size, model.UserId.ToString(), model.Status);
    }

    public static FileModel ToModel(this CFile entity)
    {
        return new FileModel() { Id = Guid.Parse(entity.Id), OriginalName = entity.OriginalName, Key = entity.Key, MimeType = entity.MimeType, Size = entity.Size, Status = entity.Status, UserId = Guid.Parse(entity.UserId) };
    }
}