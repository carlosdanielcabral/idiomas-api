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

public class ResendVerification(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IEmailVerificationTokenRepository tokenRepository,
    IEmailService emailService,
    EmailTemplateLoader templateLoader,
    IConfiguration configuration,
    ITokenHasher tokenHasher)
{
    private const int TOKEN_LENGTH = 64;

    private const int TOKEN_EXPIRATION_HOURS = 1;

    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IEmailVerificationTokenRepository _tokenRepository = tokenRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailTemplateLoader _templateLoader = templateLoader;
    private readonly IConfiguration _configuration = configuration;
    private readonly ITokenHasher _tokenHasher = tokenHasher;

    public async Task Execute(ResendVerificationDTO dto)
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

        if (user.IsEmailVerified)
        {
            return;
        }

        Guid userId = Guid.Parse(user.Id);

        await this.EnsureNoActiveTokenExists(userId);

        string rawToken = GenerateSecureToken();
        string tokenHash = this._tokenHasher.Hash(rawToken);

        EmailVerificationToken token = new(
            Guid.NewGuid(),
            userId,
            tokenHash,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(TOKEN_EXPIRATION_HOURS)
        );

        await this._tokenRepository.Insert(token);

        await this.SendVerificationEmail(user, rawToken);
    }

    private async Task EnsureNoActiveTokenExists(Guid userId)
    {
        EmailVerificationToken? activeToken = await this._tokenRepository.GetActiveTokenByUserId(userId);

        if (activeToken != null)
        {
            throw new ApiException("Já existe uma solicitação de verificação ativa. Verifique seu email ou aguarde a expiração.", HttpStatusCode.Conflict);
        }
    }

    private async Task SendVerificationEmail(User user, string rawToken)
    {
        string frontendUrl = this._configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl is not configured");
        string verificationLink = $"{frontendUrl}/verify-email?token={rawToken}";

        string htmlBody = this._templateLoader.Load("EmailVerification.html", [
            new EmailTemplatePlaceholder("UserName", user.Name),
            new EmailTemplatePlaceholder("VerificationLink", verificationLink)
        ]);

        var emailMessage = new EmailMessage(user.Email, "Verifique seu e-mail", htmlBody);

        await this._emailService.SendAsync(emailMessage);
    }

    private static string GenerateSecureToken()
    {
        return RandomNumberGenerator.GetHexString(TOKEN_LENGTH);
    }
}
