using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Presentation.DTO.Dictionary;

namespace IdiomasAPI.Source.Presentation.Mapper;

public static class WordMappingExtension
{
    public static WordResponseDTO ToResponseDTO(this Word model)
    {
        return new WordResponseDTO() { Id = model.Id.ToString(), Word = model.Name, Ipa = model.Ipa, Meanings = [.. model.Meanings.Select(m => m.ToResponseDTO())] };
    }
}