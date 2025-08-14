using Idiomas.Source.Interface.Storage;

namespace Idiomas.Source.Infrastructure.Storage;

public class FileStorage(IFileStorageAdapter adapter) : IFileStorage
{
    private readonly IFileStorageAdapter _adapter = adapter;

    public Task<string> GenerateUrlToUpload(string filename)
    {
        return this._adapter.GenerateUrlToUpload(filename);
    }
}