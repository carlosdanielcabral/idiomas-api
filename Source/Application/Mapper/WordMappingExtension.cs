using Idiomas.Source.Application.DTO.Dictionary;
using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Infrastructure.Helper;

namespace Idiomas.Source.Application.Mapper;

public static class WordMappingExtension
{
    public static Word ToEntity(this UpdateWordDTO dto, string id, string userId)
    {
        return new Word(id, dto.Word, dto.Ipa, userId, dto.Meanings.ToEntities());
    }

    public static Word ToEntity(this CreateWordDTO dto, string userId)
    {
        return new Word(UUIDGenerator.Generate(), dto.Word, dto.Ipa, userId, dto.Meanings.ToEntities());
    }
}