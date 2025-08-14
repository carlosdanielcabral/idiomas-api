using Idiomas.Core.Presentation.DTO.User;

namespace Idiomas.Core.Presentation.DTO.Auth;

public record MailPasswordLoginResponseDTO
{
    public required UserResponseDTO User { get; init; }
    public required string Token { get; init; }
}