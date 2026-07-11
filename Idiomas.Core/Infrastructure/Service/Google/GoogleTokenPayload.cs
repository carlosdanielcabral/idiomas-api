namespace Idiomas.Core.Infrastructure.Service.Google;

public record GoogleTokenPayload(
    string Subject,
    string Email,
    string Name,
    bool EmailVerified);
