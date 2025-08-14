using Idiomas.Source.Domain.Enum;

namespace Idiomas.Application.DTO.File;

public record CreateFileDTO(string OriginalFilename, string MimeType, long Size);
