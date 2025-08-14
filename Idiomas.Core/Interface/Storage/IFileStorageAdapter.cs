namespace Idiomas.Core.Interface.Storage;

public interface IFileStorageAdapter
{
    public Task<string> GenerateUrlToUpload(string filename);
}