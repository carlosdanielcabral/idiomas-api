namespace IdiomasAPI.Source.Application.DTO.Dictionary;

public record CreateMeaningDTO(string Meaning, string Example);
public record CreateWordDTO(string Word, string Ipa, List<CreateMeaningDTO> Meanings);