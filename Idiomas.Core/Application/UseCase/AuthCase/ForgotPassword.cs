using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error.Auth;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class ForgotPassword(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IPasswordResetTokenRepository tokenRepository,
    IEmailService emailService,
    EmailMessageBuilder emailMessageBuilder,
    IConfiguration configuration,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository = tokenRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailMessageBuilder _emailMessageBuilder = emailMessageBuilder;
    private readonly IConfiguration _configuration = configuration;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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

        Guid userId = user.IdAsGuid;

        await this.EnsureNoActiveTokenExists(userId);

        TokenPair token = this._tokenGenerator.Generate();

        PasswordResetToken resetToken = PasswordResetToken.Create(userId, token.TokenHash);

        await this._unitOfWork.ExecuteAsync(async () =>
        {
            await this._tokenRepository.Insert(resetToken);

            await this.SendPasswordResetEmail(user, token.RawToken);
        });
    }

    private async Task EnsureNoActiveTokenExists(Guid userId)
    {
        PasswordResetToken? activeToken = await this._tokenRepository.GetActiveTokenByUserId(userId);

        if (activeToken != null)
        {
            throw new PasswordResetRequestActiveException();
        }
    }

    private async Task SendPasswordResetEmail(User user, string rawToken)
    {
        string frontendUrl = this._configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl is not configured");
        string resetLink = $"{frontendUrl}/reset-password?token={rawToken}";

        EmailMessage emailMessage = this._emailMessageBuilder.Build(
            "PasswordResetEmail.html",
            "Redefinição de senha",
            user.Email,
            new EmailTemplatePlaceholder("UserName", user.Name),
            new EmailTemplatePlaceholder("ResetLink", resetLink)
        );

        await this._emailService.SendAsync(emailMessage);
    }
}
