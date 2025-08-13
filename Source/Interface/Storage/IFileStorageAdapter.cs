namespace IdiomasAPI.Source.Interface.Storage;

public interface IFileStorageAdapter
{
    public Task<string> GenerateUrlToUpload(string filename);
}