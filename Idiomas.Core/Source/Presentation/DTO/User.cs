namespace Idiomas.Source.Presentation.DTO.User;

public record UserResponseDTO
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}

public record CreateUserResponseDTO
{
    public required UserResponseDTO User { get; init; }
}