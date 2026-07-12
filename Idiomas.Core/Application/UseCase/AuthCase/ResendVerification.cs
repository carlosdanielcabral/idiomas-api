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
    EmailMessageBuilder emailMessageBuilder,
    IConfiguration configuration,
    ITokenGenerator tokenGenerator)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IEmailVerificationTokenRepository _tokenRepository = tokenRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailMessageBuilder _emailMessageBuilder = emailMessageBuilder;
    private readonly IConfiguration _configuration = configuration;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;

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

        Guid userId = user.IdAsGuid;

        await this.EnsureNoActiveTokenExists(userId);

        TokenPair token = this._tokenGenerator.Generate();

        EmailVerificationToken verificationToken = EmailVerificationToken.Create(userId, token.TokenHash);

        await this._tokenRepository.Insert(verificationToken);

        await this.SendVerificationEmail(user, token.RawToken);
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

        EmailMessage emailMessage = this._emailMessageBuilder.Build(
            "EmailVerification.html",
            "Verifique seu e-mail",
            user.Email,
            new EmailTemplatePlaceholder("UserName", user.Name),
            new EmailTemplatePlaceholder("VerificationLink", verificationLink)
        );

        await this._emailService.SendAsync(emailMessage);
    }
}
