using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Domain.Entity;

namespace IdiomasAPI.Source.Application.Mapper;

public static class MeaningMappingExtension
{
    public static Meaning ToEntity(this UpdateMeaningDTO dto)
    {
        return new Meaning(dto.Id.ToString(), dto.Meaning, dto.Example);
    }
    
    public static ICollection<Meaning> ToEntities(this List<UpdateMeaningDTO> dtos)
    {
        return [.. dtos.Select(dto => dto.ToEntity())];
    }
}