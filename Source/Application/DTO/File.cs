using IdiomasAPI.Source.Domain.Enum;

namespace IdiomasAPI.Application.DTO.File;

public record CreateFileDTO(string OriginalFilename, string MimeType, long Size);
public record UpdateFileDTO(FileStatus Status);