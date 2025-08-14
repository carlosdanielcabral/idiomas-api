using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Domain.Enum;
using Idiomas.Source.Infrastructure.Database.Context;
using Idiomas.Source.Infrastructure.Database.Mapper;
using Idiomas.Source.Infrastructure.Database.Model;
using Idiomas.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Source.Infrastructure.Database.Repository;

public class FileRepository(ApplicationContext database) : IFileRepository
{
    private readonly ApplicationContext _database = database;

    public async Task<CFile> Insert(CFile file)
    {
        this._database.File.Add(file.ToModel());

        await this._database.SaveChangesAsync();

        return file;
    }

    public async Task<CFile?> GetByKey(string key)
    {
        FileModel? model = await this._database.File.FirstOrDefaultAsync(f => f.Key == key);

        return model?.ToEntity();
    }

    public async Task ChangeStatus(string filekey, FileStatus status)
    {
        FileModel? model = await this._database.File.FirstOrDefaultAsync(f => f.Key == filekey);

        if (model is null)
        {
            return;
        }

        model.Status = status;

        this._database.File.Update(model);

        await this._database.SaveChangesAsync();
    }
}