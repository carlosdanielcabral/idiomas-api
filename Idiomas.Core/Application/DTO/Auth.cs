namespace Idiomas.Core.Application.DTO.Auth;

public record MailPasswordLoginDTO(string Email, string Password);

public record ForgotPasswordDTO(string Email);

public record ResetPasswordDTO(string Token, string NewPassword);