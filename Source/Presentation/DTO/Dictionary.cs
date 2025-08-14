namespace Idiomas.Source.Presentation.DTO.Dictionary;

public record MeaningResponseDTO
{
    public required string Id { get; init; }
    public required string Meaning { get; init; }
    public required string? Example { get; init; }
}

public record WordResponseDTO
{
    public required string Id { get; init; }
    public required string Word { get; init; }
    public required string Ipa { get; init; }

    public required IList<MeaningResponseDTO> Meanings { get; init; }
}

public record CreateWordResponseDTO
{
    public required WordResponseDTO Word { get; init; }
}

public record ListWordsResponseDTO
{
    public required IEnumerable<WordResponseDTO> Words { get; init; }
}
public record UpdateWordResponseDTO
{
    public required WordResponseDTO Word { get; init; }
}