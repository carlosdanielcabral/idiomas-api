using System.Net;
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

public class ResendVerificationTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserCredentialRepository> _userCredentialRepositoryMock = new();
    private readonly Mock<IEmailVerificationTokenRepository> _tokenRepositoryMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<EmailMessageBuilder> _emailMessageBuilderMock;
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock = new();

    public ResendVerificationTest()
    {
        this._emailMessageBuilderMock = new Mock<EmailMessageBuilder>(new EmailTemplateLoader(Path.Combine(Path.GetTempPath(), "fake")));
        this._configurationMock.SetupGet(config => config["FrontendUrl"]).Returns("https://app.idiomas.com");
        this._tokenGeneratorMock.Setup(generator => generator.Generate()).Returns(new TokenPair("raw-token", "hashed-token"));
        this._emailMessageBuilderMock
            .Setup(builder => builder.Build(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EmailTemplatePlaceholder[]>()))
            .Returns(new EmailMessage("joao@example.com", "subject", "<html>email</html>"));
    }

    private ResendVerification CreateSut()
    {
        return new ResendVerification(
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._tokenRepositoryMock.Object,
            this._emailServiceMock.Object,
            this._emailMessageBuilderMock.Object,
            this._configurationMock.Object,
            this._tokenGeneratorMock.Object
        );
    }

    [Fact]
    public async Task Execute_ReturnsSilentlyWhenEmailDoesNotExist()
    {
        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var sut = this.CreateSut();

        var dto = new ResendVerificationDTO("nonexistent@example.com");

        await sut.Execute(dto);

        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<EmailVerificationToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ReturnsSilentlyWhenUserHasNoLocalCredential()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", false);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync((UserCredential?)null);

        var sut = this.CreateSut();

        var dto = new ResendVerificationDTO("joao@example.com");

        await sut.Execute(dto);

        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<EmailVerificationToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ReturnsSilentlyWhenEmailAlreadyVerified()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", true);
        var credential = new UserCredential("cred-1", user.Id, AuthProvider.Local, "hashed", null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync(credential);

        var sut = this.CreateSut();

        var dto = new ResendVerificationDTO("joao@example.com");

        await sut.Execute(dto);

        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<EmailVerificationToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenActiveTokenAlreadyExists()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", false);
        var credential = new UserCredential("cred-1", user.Id, AuthProvider.Local, "hashed", null);
        var activeToken = new EmailVerificationToken(Guid.NewGuid(), Guid.Parse(user.Id), "existing-hash", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

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

        var dto = new ResendVerificationDTO("joao@example.com");

        var exception = await Assert.ThrowsAsync<ApiException>(() => sut.Execute(dto));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<EmailVerificationToken>()), Times.Never);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task Execute_GeneratesTokenAndSendsEmailWhenNoActiveTokenExists()
    {
        var user = new User(Guid.NewGuid().ToString(), "João", "joao@example.com", false);
        var credential = new UserCredential("cred-1", user.Id, AuthProvider.Local, "hashed", null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync(credential);

        this._tokenRepositoryMock
            .Setup(repository => repository.GetActiveTokenByUserId(It.IsAny<Guid>()))
            .ReturnsAsync((EmailVerificationToken?)null);

        var sut = this.CreateSut();

        var dto = new ResendVerificationDTO("joao@example.com");

        await sut.Execute(dto);

        this._tokenRepositoryMock.Verify(repository => repository.Insert(It.Is<EmailVerificationToken>(token =>
            token.UserId == Guid.Parse(user.Id) &&
            !string.IsNullOrEmpty(token.TokenHash) &&
            token.ExpiresAt > DateTime.UtcNow
        )), Times.Once);
        this._emailServiceMock.Verify(service => service.SendAsync(It.Is<EmailMessage>(message =>
            message.To == "joao@example.com"
        )), Times.Once);
    }
}
