using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Exceptions.User;
using Idiomas.Core.Application.Mapper;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;

namespace Idiomas.Core.Application.UseCase.UserCase;

public class CreateUser(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IHash hash,
    ITokenGenerator tokenGenerator,
    IEmailService emailService,
    EmailMessageBuilder emailMessageBuilder,
    IUnitOfWork unitOfWork,
    IConfiguration configuration)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository = emailVerificationTokenRepository;
    private readonly IHash _hash = hash;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailMessageBuilder _emailMessageBuilder = emailMessageBuilder;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IConfiguration _configuration = configuration;

    public async Task<User> Execute(CreateUserDTO dto)
    {
        await this.ValidateUser(dto);

        return await this._unitOfWork.ExecuteAsync(async () =>
        {
            User user = await this.CreateUserEntity(dto);

            await this.CreateLocalCredential(dto, user.Id);

            await this.CreateAndSendVerificationToken(user);

            return user;
        });
    }

    private async Task<User> CreateUserEntity(CreateUserDTO dto)
    {
        User user = dto.ToEntity();

        return await this._userRepository.Insert(user);
    }

    private async Task CreateLocalCredential(CreateUserDTO dto, string userId)
    {
        string passwordHash = this._hash.Hash(dto.Password);

        UserCredential credential = UserCredential.Create(userId, AuthProvider.Local, passwordHash);

        await this._userCredentialRepository.Insert(credential);
    }

    private async Task CreateAndSendVerificationToken(User user)
    {
        TokenPair token = this._tokenGenerator.Generate();

        EmailVerificationToken verificationToken = EmailVerificationToken.Create(user.IdAsGuid, token.TokenHash);

        await this._emailVerificationTokenRepository.Insert(verificationToken);

        await this.SendVerificationEmail(user, token.RawToken);
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

    private async Task ValidateUser(CreateUserDTO dto)
    {
        User? existingUser = await this._userRepository.GetByEmail(dto.Email);

        if (existingUser != null)
        {
            throw new EmailAlreadyInUseException();
        }
    }
}
