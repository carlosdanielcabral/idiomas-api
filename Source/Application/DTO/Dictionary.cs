namespace IdiomasAPI.Source.Application.DTO.Dictionary;

public record CreateMeaningDTO(string Meaning, string Example);
public record UpdateMeaningDTO(string Id, string Meaning, string Example);
public record CreateWordDTO(string Word, string Ipa, List<CreateMeaningDTO> Meanings);
public record UpdateWordDTO(string Id, string Word, string Ipa, List<UpdateMeaningDTO> Meanings);