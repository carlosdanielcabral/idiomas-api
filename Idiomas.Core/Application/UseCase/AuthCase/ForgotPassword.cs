using System.Security.Cryptography;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class ForgotPassword(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IPasswordResetTokenRepository tokenRepository,
    IEmailService emailService,
    EmailTemplateLoader templateLoader,
    IConfiguration configuration,
    ITokenHasher tokenHasher)
{
    private const int TOKEN_LENGTH = 64;

    private const int TOKEN_EXPIRATION_HOURS = 1;

    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository = tokenRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailTemplateLoader _templateLoader = templateLoader;
    private readonly IConfiguration _configuration = configuration;
    private readonly ITokenHasher _tokenHasher = tokenHasher;

    public async Task Execute(ForgotPasswordDTO dto)
    {
        User? user = await this._userRepository.GetByEmail(dto.Email);

        if (user == null)
        {
            return;
        }

        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(user.Id, AuthProvider.Local);

        if (credential == null)
        {
            return;
        }

        Guid userId = Guid.Parse(user.Id);

        await this.EnsureNoActiveTokenExists(userId);

        string rawToken = GenerateSecureToken();
        string tokenHash = this._tokenHasher.Hash(rawToken);

        PasswordResetToken token = new(Guid.NewGuid(), userId, tokenHash, DateTime.UtcNow, DateTime.UtcNow.AddHours(TOKEN_EXPIRATION_HOURS));

        await this._tokenRepository.Insert(token);

        await this.SendPasswordResetEmail(user, rawToken);
    }

    private async Task EnsureNoActiveTokenExists(Guid userId)
    {
        PasswordResetToken? activeToken = await this._tokenRepository.GetActiveTokenByUserId(userId);

        if (activeToken != null)
        {
            throw new ApiException("Já existe uma solicitação de redefinição de senha ativa. Verifique seu email ou aguarde a expiração.", HttpStatusCode.Conflict);
        }
    }

    private async Task SendPasswordResetEmail(User user, string rawToken)
    {
        string frontendUrl = this._configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl is not configured");
        string resetLink = $"{frontendUrl}/reset-password?token={rawToken}";

        string htmlBody = this._templateLoader.Load("PasswordResetEmail.html", [
            new EmailTemplatePlaceholder("UserName", user.Name),
            new EmailTemplatePlaceholder("ResetLink", resetLink)
        ]);

        var emailMessage = new EmailMessage(user.Email, "Redefinição de senha", htmlBody);

        await this._emailService.SendAsync(emailMessage);
    }

    private static string GenerateSecureToken()
    {
        return RandomNumberGenerator.GetHexString(TOKEN_LENGTH);
    }
}
