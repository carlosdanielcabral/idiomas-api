using Idiomas.Application.DTO.File;
using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Helper;

namespace Idiomas.Core.Application.Mapper;

public static class FileMappingExtension
{
    public static CFile ToEntity(this CreateFileDTO dto, string key, string userId)
    {
        return new CFile(UUIDGenerator.Generate(), dto.OriginalFilename, key, dto.MimeType, dto.Size, userId);
    }
}