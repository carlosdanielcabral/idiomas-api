using IdiomasAPI.Application.DTO.File;
using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Helper;

namespace IdiomasAPI.Source.Application.Mapper;

public static class FileMappingExtension
{
    public static CFile ToEntity(this CreateFileDTO dto, string key, string userId)
    {
        return new CFile(UUIDGenerator.Generate(), dto.OriginalFilename, key, dto.MimeType, dto.Size, userId);
    }
}