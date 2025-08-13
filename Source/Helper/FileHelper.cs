namespace IdiomasAPI.Source.Helper;

public class FileHelper
{
    public string GenerateFileKey(string originalFilename)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        return $"{timestamp}{Guid.NewGuid().ToString("N")}{Path.GetExtension(originalFilename)}";
    }
}