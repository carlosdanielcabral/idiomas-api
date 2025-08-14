using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Presentation.DTO.Dictionary;
using Idiomas.Source.Presentation.DTO.User;

namespace Idiomas.Source.Presentation.Mapper;

public static class MeaningMappingExtension
{
    public static MeaningResponseDTO ToResponseDTO(this Meaning model)
    {
        return new MeaningResponseDTO() { Id = model.Id.ToString(), Meaning = model.Definition, Example = model.Example };
    }
}