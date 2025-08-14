using Idiomas.Core.Domain.Entity;
using Idiomas.Api.DTO.Dictionary;
using Idiomas.Api.DTO.User;

namespace Idiomas.Api.Mapper;

public static class MeaningMappingExtension
{
    public static MeaningResponseDTO ToResponseDTO(this Meaning model)
    {
        return new MeaningResponseDTO() { Id = model.Id.ToString(), Meaning = model.Definition, Example = model.Example };
    }
}