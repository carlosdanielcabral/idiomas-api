using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Presentation.DTO.Dictionary;

namespace Idiomas.Core.Presentation.Mapper;

public static class WordMappingExtension
{
    public static WordResponseDTO ToResponseDTO(this Word model)
    {
        return new WordResponseDTO() { Id = model.Id.ToString(), Word = model.Name, Ipa = model.Ipa, Meanings = [.. model.Meanings.Select(m => m.ToResponseDTO())] };
    }

    public static IEnumerable<WordResponseDTO> ToResponseDTO(this IEnumerable<Word> models)
    {
        return models.Select(model => model.ToResponseDTO());
    }
}