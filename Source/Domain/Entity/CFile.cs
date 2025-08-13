using IdiomasAPI.Source.Domain.Enum;

namespace IdiomasAPI.Source.Domain.Entity;

public class CFile(string id, string originalName, string key, string mimeType, long size, string userId, FileStatus status = FileStatus.Pending)
{
    public string Id { get; private set; } = id;
    public string OriginalName { get; private set; } = originalName;
    public string Key { get; private set; } = key;
    public string MimeType { get; private set; } = mimeType;
    public long Size { get; private set; } = size;
    public FileStatus Status { get; set; } = status;

    public string UserId { get; set; } = userId;
}
