using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Application.Mapper;

public static class WordMappingExtension
{
    public static Word ToEntity(this UpdateWordDTO dto, string userId)
    {
        return new Word(dto.Id.ToString(), dto.Word, dto.Ipa, userId, dto.Meanings.ToEntities());
    }
}