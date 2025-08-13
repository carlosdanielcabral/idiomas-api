using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Presentation.DTO.Dictionary;
using IdiomasAPI.Source.Presentation.DTO.User;

namespace IdiomasAPI.Source.Presentation.Mapper;

public static class MeaningMappingExtension
{
    public static MeaningResponseDTO ToResponseDTO(this Meaning model)
    {
        return new MeaningResponseDTO() { Id = model.Id.ToString(), Meaning = model.Definition, Example = model.Example };
    }
}