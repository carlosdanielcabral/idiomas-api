using Idiomas.Source.Domain.Entity;

namespace Idiomas.Source.Interface.Repository;

public interface IFileRepository
{
    public Task<CFile> Insert(CFile file);
    public Task<CFile?> GetByKey(string key);
    public Task ChangeStatus(string filekey, Domain.Enum.FileStatus status);
}