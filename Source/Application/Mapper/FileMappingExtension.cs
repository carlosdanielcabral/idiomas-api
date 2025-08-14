using Idiomas.Application.DTO.File;
using Idiomas.Source.Application.DTO.Dictionary;
using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Infrastructure.Helper;

namespace Idiomas.Source.Application.Mapper;

public static class FileMappingExtension
{
    public static CFile ToEntity(this CreateFileDTO dto, string key, string userId)
    {
        return new CFile(UUIDGenerator.Generate(), dto.OriginalFilename, key, dto.MimeType, dto.Size, userId);
    }
}