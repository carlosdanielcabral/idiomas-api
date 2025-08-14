using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Presentation.DTO.Dictionary;
using Idiomas.Core.Presentation.DTO.User;

namespace Idiomas.Core.Presentation.Mapper;

public static class MeaningMappingExtension
{
    public static MeaningResponseDTO ToResponseDTO(this Meaning model)
    {
        return new MeaningResponseDTO() { Id = model.Id.ToString(), Meaning = model.Definition, Example = model.Example };
    }
}