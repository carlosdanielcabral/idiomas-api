using System.Net;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Exceptions.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.Google;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.AuthCase;

public class GoogleLoginTest
{
    private readonly Mock<IGoogleTokenVerifier> _tokenVerifierMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserCredentialRepository> _userCredentialRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GoogleLogin _sut;

    public GoogleLoginTest()
    {
        this._tokenVerifierMock = new Mock<IGoogleTokenVerifier>();
        this._userRepositoryMock = new Mock<IUserRepository>();
        this._userCredentialRepositoryMock = new Mock<IUserCredentialRepository>();
        this._unitOfWorkMock = new Mock<IUnitOfWork>();

        this._unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.ExecuteAsync(It.IsAny<Func<Task<User>>>()))
            .Returns((Func<Task<User>> operation) => operation());

        this._sut = new GoogleLogin(
            this._tokenVerifierMock.Object,
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._unitOfWorkMock.Object
        );
    }

    private GoogleTokenPayload CreatePayload(string? subject = "google-sub-123", bool emailVerified = true)
    {
        return new GoogleTokenPayload(subject!, "joao@gmail.com", "João Silva", emailVerified);
    }

    [Fact]
    public async Task Execute_ThrowsGoogleEmailNotVerifiedExceptionWhenEmailIsNotVerified()
    {
        GoogleTokenPayload payload = CreatePayload(emailVerified: false);

        this._tokenVerifierMock
            .Setup(verifier => verifier.Verify(It.IsAny<string>()))
            .ReturnsAsync(payload);

        var dto = new GoogleLoginDTO("id-token");

        var exception = await Assert.ThrowsAsync<GoogleEmailNotVerifiedException>(() => this._sut.Execute(dto));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("auth:google-email-not-verified", exception.ErrorCode);
        Assert.Equal("Google email not verified", exception.Title);
        Assert.Equal("The email has not been verified by Google.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ReturnsUserWhenCredentialAlreadyLinked()
    {
        GoogleTokenPayload payload = CreatePayload();
        User user = new("user-1", "João Silva", "joao@gmail.com", true);
        UserCredential credential = new("cred-1", "user-1", AuthProvider.Google, null, "google-sub-123");

        this._tokenVerifierMock
            .Setup(verifier => verifier.Verify(It.IsAny<string>()))
            .ReturnsAsync(payload);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByExternalSubject(AuthProvider.Google, payload.Subject))
            .ReturnsAsync(credential);

        this._userRepositoryMock
            .Setup(repository => repository.GetById("user-1"))
            .ReturnsAsync(user);

        var dto = new GoogleLoginDTO("id-token");

        var result = await this._sut.Execute(dto);

        Assert.Equal(user, result);

        this._userRepositoryMock.Verify(repository => repository.Insert(It.IsAny<User>()), Times.Never);
        this._userCredentialRepositoryMock.Verify(repository => repository.Insert(It.IsAny<UserCredential>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ThrowsAccountNotFoundExceptionWhenCredentialExistsButUserDoesNot()
    {
        GoogleTokenPayload payload = CreatePayload();
        UserCredential credential = new("cred-1", "user-1", AuthProvider.Google, null, "google-sub-123");

        this._tokenVerifierMock
            .Setup(verifier => verifier.Verify(It.IsAny<string>()))
            .ReturnsAsync(payload);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByExternalSubject(AuthProvider.Google, payload.Subject))
            .ReturnsAsync(credential);

        this._userRepositoryMock
            .Setup(repository => repository.GetById("user-1"))
            .ReturnsAsync((User?)null);

        var dto = new GoogleLoginDTO("id-token");

        var exception = await Assert.ThrowsAsync<AccountNotFoundException>(() => this._sut.Execute(dto));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("auth:account-not-found", exception.ErrorCode);
        Assert.Equal("Account not found", exception.Title);
        Assert.Equal("No account was found for the provided credentials.", exception.Detail);
    }

    [Fact]
    public async Task Execute_LinksGoogleCredentialWhenUserExistsByEmail()
    {
        GoogleTokenPayload payload = CreatePayload();
        User existingUser = new("user-1", "João Silva", "joao@gmail.com", true);

        this._tokenVerifierMock
            .Setup(verifier => verifier.Verify(It.IsAny<string>()))
            .ReturnsAsync(payload);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByExternalSubject(AuthProvider.Google, payload.Subject))
            .ReturnsAsync((UserCredential?)null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(payload.Email))
            .ReturnsAsync(existingUser);

        this._userRepositoryMock
            .Setup(repository => repository.GetById("user-1"))
            .ReturnsAsync(existingUser);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<UserCredential>()))
            .ReturnsAsync((UserCredential credential) => credential);

        var dto = new GoogleLoginDTO("id-token");

        var result = await this._sut.Execute(dto);

        Assert.Equal(existingUser, result);

        this._userCredentialRepositoryMock.Verify(repository => repository.Insert(It.Is<UserCredential>(credential => credential.Provider == AuthProvider.Google && credential.ExternalSubject == "google-sub-123" && credential.PasswordHash == null)), Times.Once);
        this._userRepositoryMock.Verify(repository => repository.Insert(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Execute_CreatesNewUserAndCredentialWhenNoExistingAccount()
    {
        GoogleTokenPayload payload = CreatePayload();

        this._tokenVerifierMock
            .Setup(verifier => verifier.Verify(It.IsAny<string>()))
            .ReturnsAsync(payload);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByExternalSubject(AuthProvider.Google, payload.Subject))
            .ReturnsAsync((UserCredential?)null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(payload.Email))
            .ReturnsAsync((User?)null);

        this._userRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<UserCredential>()))
            .ReturnsAsync((UserCredential credential) => credential);

        var dto = new GoogleLoginDTO("id-token");

        var result = await this._sut.Execute(dto);

        Assert.Equal("João Silva", result.Name);
        Assert.Equal("joao@gmail.com", result.Email);

        this._userRepositoryMock.Verify(repository => repository.Insert(It.Is<User>(user => user.Name == "João Silva" && user.Email == "joao@gmail.com")), Times.Once);
        this._userCredentialRepositoryMock.Verify(repository => repository.Insert(It.Is<UserCredential>(credential => credential.Provider == AuthProvider.Google && credential.ExternalSubject == "google-sub-123" && credential.PasswordHash == null)), Times.Once);
    }

    [Fact]
    public async Task Execute_ThrowsEmailNotVerifiedExceptionWhenLocalAccountNotVerified()
    {
        GoogleTokenPayload payload = CreatePayload();
        User existingUser = new("user-1", "João Silva", "joao@gmail.com", false);

        this._tokenVerifierMock
            .Setup(verifier => verifier.Verify(It.IsAny<string>()))
            .ReturnsAsync(payload);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByExternalSubject(AuthProvider.Google, payload.Subject))
            .ReturnsAsync((UserCredential?)null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(payload.Email))
            .ReturnsAsync(existingUser);

        var dto = new GoogleLoginDTO("id-token");

        var exception = await Assert.ThrowsAsync<EmailNotVerifiedException>(() => this._sut.Execute(dto));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        Assert.Equal("auth:email-not-verified", exception.ErrorCode);
        Assert.Equal("Email not verified", exception.Title);
        Assert.Equal("The email has not been verified. Check your inbox to activate your account.", exception.Detail);
    }
}
