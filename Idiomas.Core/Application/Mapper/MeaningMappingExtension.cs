using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Helper;

namespace Idiomas.Core.Application.Mapper;

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