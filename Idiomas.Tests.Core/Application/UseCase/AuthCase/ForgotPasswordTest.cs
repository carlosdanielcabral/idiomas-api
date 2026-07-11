using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.AuthCase;

public class ForgotPasswordTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserCredentialRepository> _userCredentialRepositoryMock = new();
    private readonly Mock<IPasswordResetTokenRepository> _tokenRepositoryMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<EmailTemplateLoader> _templateLoaderMock;
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<ITokenHasher> _tokenHasherMock = new();

    public ForgotPasswordTest()
    {
        this._templateLoaderMock = new Mock<EmailTemplateLoader>(Path.Combine(Path.GetTempPath(), "fake"));
        this._configurationMock.SetupGet(config => config["FrontendUrl"]).Returns("https://app.idiomas.com");
        this._tokenHasherMock.Setup(hasher => hasher.Hash(It.IsAny<string>())).Returns("hashed-token");
    }

    private ForgotPassword CreateSut()
    {
        return new ForgotPassword(
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._tokenRepositoryMock.Object,
            this._emailServiceMock.Object,
            this._templateLoaderMock.Object,
            this._configurationMock.Object,
            this._tokenHasherMock.Object
        );
    }

    [Fact]
    public async Task Execute_ReturnsSilentlyWhenEmailDoesNotExist()
    {
        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var sut = this.CreateSut();

        var dto = new ForgotPasswordDTO("nonexistent@example.com");

        await sut.Execute(dto);

        this._tokenRepositoryMock.Verify(repository => repository.GetActiveTokenByUserId(It.IsAny<Guid>()), Times.Never);
        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<PasswordResetToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ReturnsSilentlyWhenUserHasNoLocalCredential()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", true);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync((UserCredential?)null);

        var sut = this.CreateSut();

        var dto = new ForgotPasswordDTO("joao@example.com");

        await sut.Execute(dto);

        this._tokenRepositoryMock.Verify(repository => repository.GetActiveTokenByUserId(It.IsAny<Guid>()), Times.Never);
        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<PasswordResetToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenActiveTokenAlreadyExists()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", true);
        var credential = new UserCredential("cred-1", user.Id, AuthProvider.Local, "hashed", null);
        var activeToken = new PasswordResetToken(Guid.NewGuid(), Guid.Parse(user.Id), "existing-hash", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync(credential);

        this._tokenRepositoryMock
            .Setup(repository => repository.GetActiveTokenByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(activeToken);

        var sut = this.CreateSut();

        var dto = new ForgotPasswordDTO("joao@example.com");

        var exception = await Assert.ThrowsAsync<ApiException>(() => sut.Execute(dto));

        Assert.Equal(System.Net.HttpStatusCode.Conflict, exception.StatusCode);
        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<PasswordResetToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_GeneratesTokenAndSendsEmailWhenNoActiveTokenExists()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", true);
        var credential = new UserCredential("cred-1", user.Id, AuthProvider.Local, "hashed", null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync(credential);

        this._tokenRepositoryMock
            .Setup(repository => repository.GetActiveTokenByUserId(It.IsAny<Guid>()))
            .ReturnsAsync((PasswordResetToken?)null);

        this._templateLoaderMock
            .Setup(loader => loader.Load(It.IsAny<string>(), It.IsAny<IEnumerable<EmailTemplatePlaceholder>>()))
            .Returns("<html>email</html>");

        var sut = this.CreateSut();

        var dto = new ForgotPasswordDTO("joao@example.com");

        await sut.Execute(dto);

        this._tokenHasherMock.Verify(hasher => hasher.Hash(It.IsAny<string>()), Times.Once);
        this._tokenRepositoryMock.Verify(repository => repository.GetActiveTokenByUserId(Guid.Parse(user.Id)), Times.Once);
        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.Is<PasswordResetToken>(token =>
            token.UserId == Guid.Parse(user.Id) &&
            !string.IsNullOrEmpty(token.TokenHash) &&
            token.ExpiresAt > DateTime.UtcNow
        )), Times.Once);
        this._emailServiceMock.Verify(service => service.SendAsync(It.Is<EmailMessage>(message =>
            message.To == "joao@example.com"
        )), Times.Once);
    }
}
