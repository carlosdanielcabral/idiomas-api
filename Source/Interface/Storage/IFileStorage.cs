namespace Idiomas.Source.Interface.Storage;

public interface IFileStorage
{
    public Task<string> GenerateUrlToUpload(string filename);
}