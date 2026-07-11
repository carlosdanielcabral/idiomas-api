using System.Net;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.UserCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.UserCase;

public class CreateUserTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserCredentialRepository> _userCredentialRepositoryMock;
    private readonly Mock<IEmailVerificationTokenRepository> _emailVerificationTokenRepositoryMock;
    private readonly Mock<IHash> _hashMock;
    private readonly Mock<ITokenHasher> _tokenHasherMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<EmailTemplateLoader> _templateLoaderMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ITransactionManager> _transactionManagerMock;
    private readonly Mock<IDatabaseTransaction> _databaseTransactionMock;
    private readonly CreateUser _sut;

    public CreateUserTest()
    {
        this._userRepositoryMock = new Mock<IUserRepository>();
        this._userCredentialRepositoryMock = new Mock<IUserCredentialRepository>();
        this._emailVerificationTokenRepositoryMock = new Mock<IEmailVerificationTokenRepository>();
        this._hashMock = new Mock<IHash>();
        this._tokenHasherMock = new Mock<ITokenHasher>();
        this._emailServiceMock = new Mock<IEmailService>();
        this._templateLoaderMock = new Mock<EmailTemplateLoader>(Path.Combine(Path.GetTempPath(), "fake"));
        this._configurationMock = new Mock<IConfiguration>();
        this._transactionManagerMock = new Mock<ITransactionManager>();
        this._databaseTransactionMock = new Mock<IDatabaseTransaction>();

        this._transactionManagerMock
            .Setup(manager => manager.BeginTransactionAsync())
            .ReturnsAsync(this._databaseTransactionMock.Object);

        this._configurationMock.SetupGet(config => config["FrontendUrl"]).Returns("https://app.idiomas.com");
        this._tokenHasherMock.Setup(hasher => hasher.Hash(It.IsAny<string>())).Returns("hashed-token");
        this._templateLoaderMock
            .Setup(loader => loader.Load(It.IsAny<string>(), It.IsAny<IEnumerable<EmailTemplatePlaceholder>>()))
            .Returns("<html>email</html>");

        this._sut = new CreateUser(
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._emailVerificationTokenRepositoryMock.Object,
            this._hashMock.Object,
            this._tokenHasherMock.Object,
            this._emailServiceMock.Object,
            this._templateLoaderMock.Object,
            this._transactionManagerMock.Object,
            this._configurationMock.Object
        );
    }

    [Fact]
    public async Task Execute_ShouldCreateUser_WhenEmailIsUnique()
    {
        var createUserDTO = new CreateUserDTO("Test User", "test@example.com", "password123");

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        this._hashMock
            .Setup(hash => hash.Hash(It.IsAny<string>()))
            .Returns("hashed_password");

        this._userRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        var result = await this._sut.Execute(createUserDTO);

        Assert.NotNull(result);
        Assert.Equal(createUserDTO.Name, result.Name);
        Assert.Equal(createUserDTO.Email, result.Email);

        this._userRepositoryMock.Verify(repository => repository.Insert(It.IsAny<User>()), Times.Once);
        this._userCredentialRepositoryMock.Verify(repository => repository.Insert(It.IsAny<UserCredential>()), Times.Once);
        this._emailVerificationTokenRepositoryMock.Verify(repository => repository.Insert(It.IsAny<EmailVerificationToken>()), Times.Once);
        this._emailServiceMock.Verify(service => service.SendAsync(It.IsAny<EmailMessage>()), Times.Once);
        this._databaseTransactionMock.Verify(transaction => transaction.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowApiException_WhenEmailAlreadyExists()
    {
        CreateUserDTO createUserDTO = new("Test User", "test@example.com", "password123");
        User existingUser = new("1", "Existing User", "test@example.com", true);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(createUserDTO.Email))
            .ReturnsAsync(existingUser);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(createUserDTO));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Equal("E-mail já cadastrado", exception.Message);

        this._userRepositoryMock.Verify(repository => repository.Insert(It.IsAny<User>()), Times.Never);
    }
}
