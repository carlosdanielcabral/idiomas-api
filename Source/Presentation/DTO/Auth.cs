using Idiomas.Source.Presentation.DTO.User;

namespace Idiomas.Source.Presentation.DTO.Auth;

public record MailPasswordLoginResponseDTO
{
    public required UserResponseDTO User { get; init; }
    public required string Token { get; init; }
}