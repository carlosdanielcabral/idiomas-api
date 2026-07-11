using System.Security.Cryptography;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Idiomas.Core.Application.UseCase.UserCase;

public class UpdateUser(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IEmailChangeRequestRepository emailChangeRequestRepository,
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
    private readonly IEmailChangeRequestRepository _emailChangeRequestRepository = emailChangeRequestRepository;
    private readonly IHash _hash = hash;
    private readonly ITokenHasher _tokenHasher = tokenHasher;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailTemplateLoader _templateLoader = templateLoader;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<User> Execute(string userId, UpdateUserDTO dto)
    {
        User? currentUser = await this._userRepository.GetById(userId);

        if (currentUser is null)
        {
            throw new ApiException("Usuário não encontrado", HttpStatusCode.NotFound);
        }

        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        User updatedUser = await this.UpdateUserProfile(userId, dto, currentUser);

        if (this.IsEmailChanging(dto, currentUser))
        {
            await this.ValidateNewEmail(dto.Email);

            await this.CreateEmailChangeRequest(currentUser, dto.Email);
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            await this.UpdateUserPassword(userId, dto.Password);
        }

        await transaction.CommitAsync();

        return updatedUser;
    }

    private async Task<User> UpdateUserProfile(string userId, UpdateUserDTO dto, User currentUser)
    {
        User updatedUser = new(userId, dto.Name, currentUser.Email, currentUser.IsEmailVerified);

        return await this._userRepository.Update(updatedUser);
    }

    private bool IsEmailChanging(UpdateUserDTO dto, User currentUser)
    {
        return !string.Equals(dto.Email, currentUser.Email, StringComparison.OrdinalIgnoreCase);
    }

    private async Task ValidateNewEmail(string newEmail)
    {
        User? existingUser = await this._userRepository.GetByEmail(newEmail);

        if (existingUser is not null)
        {
            throw new ApiException("E-mail já cadastrado por outro usuário", HttpStatusCode.Conflict);
        }
    }

    private async Task CreateEmailChangeRequest(User currentUser, string newEmail)
    {
        Guid userId = Guid.Parse(currentUser.Id);

        await this.EnsureNoActiveChangeRequestExists(userId);

        string rawToken = GenerateSecureToken();
        string tokenHash = this._tokenHasher.Hash(rawToken);

        EmailChangeRequest request = new(
            Guid.NewGuid(),
            userId,
            newEmail,
            tokenHash,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(TOKEN_EXPIRATION_HOURS)
        );

        await this._emailChangeRequestRepository.Insert(request);

        await this.SendEmailChangeConfirmation(newEmail, currentUser.Name, rawToken);
    }

    private async Task EnsureNoActiveChangeRequestExists(Guid userId)
    {
        EmailChangeRequest? activeRequest = await this._emailChangeRequestRepository.GetActiveRequestByUserId(userId);

        if (activeRequest != null)
        {
            throw new ApiException("Já existe uma solicitação de troca de e-mail ativa. Verifique seu email ou aguarde a expiração.", HttpStatusCode.Conflict);
        }
    }

    private async Task SendEmailChangeConfirmation(string newEmail, string userName, string rawToken)
    {
        string frontendUrl = this._configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl is not configured");
        string confirmationLink = $"{frontendUrl}/verify-email-change?token={rawToken}";

        string htmlBody = this._templateLoader.Load("EmailChangeConfirmation.html", [
            new EmailTemplatePlaceholder("UserName", userName),
            new EmailTemplatePlaceholder("ConfirmationLink", confirmationLink)
        ]);

        var emailMessage = new EmailMessage(newEmail, "Confirme seu novo e-mail", htmlBody);

        await this._emailService.SendAsync(emailMessage);
    }

    private async Task UpdateUserPassword(string userId, string password)
    {
        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(userId, AuthProvider.Local);

        if (credential is null)
        {
            throw new ApiException("Usuário não possui credencial local", HttpStatusCode.BadRequest);
        }

        string passwordHash = this._hash.Hash(password);
        credential.UpdatePasswordHash(passwordHash);

        await this._userCredentialRepository.Update(credential);
    }

    private static string GenerateSecureToken()
    {
        return RandomNumberGenerator.GetHexString(TOKEN_LENGTH);
    }
}
