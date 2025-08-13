using IdiomasAPI.Source.Application.DTO.Dictionary;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Helper;

namespace IdiomasAPI.Source.Application.Mapper;

public static class MeaningMappingExtension
{
    public static Meaning ToEntity(this CreateMeaningDTO dto)
    {
        return new Meaning(UUIDGenerator.Generate(), dto.Meaning, dto.Example);
    }

    public static ICollection<Meaning> ToEntities(this List<CreateMeaningDTO> dtos)
    {
        return [.. dtos.Select(dto => dto.ToEntity())];
    }
}