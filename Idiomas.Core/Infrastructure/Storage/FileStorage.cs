using Idiomas.Core.Interface.Storage;

namespace Idiomas.Core.Infrastructure.Storage;

public class FileStorage(IFileStorageAdapter adapter) : IFileStorage
{
    private readonly IFileStorageAdapter _adapter = adapter;

    public Task<string> GenerateUrlToUpload(string filename)
    {
        return this._adapter.GenerateUrlToUpload(filename);
    }
}