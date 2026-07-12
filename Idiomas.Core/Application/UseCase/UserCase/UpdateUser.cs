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
    ITokenGenerator tokenGenerator,
    IEmailService emailService,
    EmailMessageBuilder emailMessageBuilder,
    ITransactionManager transactionManager,
    IConfiguration configuration)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IEmailChangeRequestRepository _emailChangeRequestRepository = emailChangeRequestRepository;
    private readonly IHash _hash = hash;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailMessageBuilder _emailMessageBuilder = emailMessageBuilder;
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

        currentUser.UpdateProfile(dto.Name);

        if (currentUser.IsEmailChanging(dto.Email))
        {
            await this.ChangeUserEmail(currentUser, dto.Email);
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            await this.UpdateUserPassword(userId, dto.Password);
        }

        User updatedUser = await this._userRepository.Update(currentUser);

        await transaction.CommitAsync();

        return updatedUser;
    }

    private async Task ChangeUserEmail(User currentUser, string newEmail)
    {
        await this.ValidateNewEmail(newEmail);

        await this.CreateEmailChangeRequest(currentUser, newEmail);

        currentUser.UpdateEmail(newEmail);
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
        Guid userId = currentUser.IdAsGuid;

        await this.EnsureNoActiveChangeRequestExists(userId);

        TokenPair token = this._tokenGenerator.Generate();

        EmailChangeRequest request = EmailChangeRequest.Create(userId, newEmail, token.TokenHash);

        await this._emailChangeRequestRepository.Insert(request);

        await this.SendEmailChangeConfirmation(newEmail, currentUser.Name, token.RawToken);
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

        EmailMessage emailMessage = this._emailMessageBuilder.Build(
            "EmailChangeConfirmation.html",
            "Confirme seu novo e-mail",
            newEmail,
            new EmailTemplatePlaceholder("UserName", userName),
            new EmailTemplatePlaceholder("ConfirmationLink", confirmationLink)
        );

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
}
