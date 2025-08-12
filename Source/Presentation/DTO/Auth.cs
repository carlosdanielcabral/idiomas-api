using IdiomasAPI.Source.Presentation.DTO.User;

namespace IdiomasAPI.Source.Presentation.DTO.Auth;

public record MailPasswordLoginResponseDTO
{
    public required UserResponseDTO User { get; init; }
    public required string Token { get; init; }
}