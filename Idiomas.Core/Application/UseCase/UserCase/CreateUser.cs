using System.Security.Cryptography;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.Mapper;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Idiomas.Core.Application.UseCase.UserCase;

public class CreateUser(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IHash hash,
    ITokenHasher tokenHasher,
    IEmailService emailService,
    EmailTemplateLoader templateLoader,
    ITransactionManager transactionManager,
    IConfiguration configuration)
{
    private const int TOKEN_LENGTH = 64;

    private const int TOKEN_EXPIRATION_HOURS = 1;

    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository = emailVerificationTokenRepository;
    private readonly IHash _hash = hash;
    private readonly ITokenHasher _tokenHasher = tokenHasher;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailTemplateLoader _templateLoader = templateLoader;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<User> Execute(CreateUserDTO dto)
    {
        await this.ValidateUser(dto);

        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        User user = await this.CreateUserEntity(dto);

        await this.CreateLocalCredential(dto, user.Id);

        await this.CreateAndSendVerificationToken(user);

        await transaction.CommitAsync();

        return user;
    }

    private async Task<User> CreateUserEntity(CreateUserDTO dto)
    {
        User user = dto.ToEntity();

        return await this._userRepository.Insert(user);
    }

    private async Task CreateLocalCredential(CreateUserDTO dto, string userId)
    {
        string passwordHash = this._hash.Hash(dto.Password);

        UserCredential credential = dto.ToCredentialEntity(userId, passwordHash);

        await this._userCredentialRepository.Insert(credential);
    }

    private async Task CreateAndSendVerificationToken(User user)
    {
        string rawToken = GenerateSecureToken();
        string tokenHash = this._tokenHasher.Hash(rawToken);

        EmailVerificationToken token = new(
            Guid.NewGuid(),
            Guid.Parse(user.Id),
            tokenHash,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(TOKEN_EXPIRATION_HOURS)
        );

        await this._emailVerificationTokenRepository.Insert(token);

        await this.SendVerificationEmail(user, rawToken);
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

    private async Task ValidateUser(CreateUserDTO dto)
    {
        User? existingUser = await this._userRepository.GetByEmail(dto.Email);

        if (existingUser != null)
        {
            throw new ApiException("E-mail já cadastrado", HttpStatusCode.Conflict);
        }
    }

    private static string GenerateSecureToken()
    {
        return RandomNumberGenerator.GetHexString(TOKEN_LENGTH);
    }
}
