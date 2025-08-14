using Idiomas.Api.DTO.User;

namespace Idiomas.Api.DTO.Auth;

public record MailPasswordLoginResponseDTO
{
    public required UserResponseDTO User { get; init; }
    public required string Token { get; init; }
}