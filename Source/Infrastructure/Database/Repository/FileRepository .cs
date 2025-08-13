using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Domain.Enum;
using IdiomasAPI.Source.Infrastructure.Database.Context;
using IdiomasAPI.Source.Infrastructure.Database.Mapper;
using IdiomasAPI.Source.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace IdiomasAPI.Source.Infrastructure.Database.Repository;

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
        var model = await this._database.File.FirstOrDefaultAsync(f => f.Key == key);

        return model?.ToEntity();
    }

    public async Task ChangeStatus(string filekey, FileStatus status)
    {
        var model = await this._database.File.FirstOrDefaultAsync(f => f.Key == filekey);

        if (model == null)
        {
            return;
        }

        model.Status = status;

        this._database.File.Update(model);

        await this._database.SaveChangesAsync();
    }
}