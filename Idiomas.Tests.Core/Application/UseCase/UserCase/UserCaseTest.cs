using System.Net;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error.User;
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
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<EmailMessageBuilder> _emailMessageBuilderMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateUser _sut;

    public CreateUserTest()
    {
        this._userRepositoryMock = new Mock<IUserRepository>();
        this._userCredentialRepositoryMock = new Mock<IUserCredentialRepository>();
        this._emailVerificationTokenRepositoryMock = new Mock<IEmailVerificationTokenRepository>();
        this._hashMock = new Mock<IHash>();
        this._tokenGeneratorMock = new Mock<ITokenGenerator>();
        this._emailServiceMock = new Mock<IEmailService>();
        this._emailMessageBuilderMock = new Mock<EmailMessageBuilder>(new EmailTemplateLoader(Path.Combine(Path.GetTempPath(), "fake")));
        this._configurationMock = new Mock<IConfiguration>();
        this._unitOfWorkMock = new Mock<IUnitOfWork>();

        this._unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.ExecuteAsync(It.IsAny<Func<Task<User>>>()))
            .Returns((Func<Task<User>> operation) => operation());

        this._configurationMock.SetupGet(config => config["FrontendUrl"]).Returns("https://app.idiomas.com");
        this._tokenGeneratorMock.Setup(generator => generator.Generate()).Returns(new TokenPair("raw-token", "hashed-token"));
        this._emailMessageBuilderMock
            .Setup(builder => builder.Build(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EmailTemplatePlaceholder[]>()))
            .Returns(new EmailMessage("test@example.com", "subject", "<html>email</html>"));

        this._sut = new CreateUser(
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._emailVerificationTokenRepositoryMock.Object,
            this._hashMock.Object,
            this._tokenGeneratorMock.Object,
            this._emailServiceMock.Object,
            this._emailMessageBuilderMock.Object,
            this._unitOfWorkMock.Object,
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
    }

    [Fact]
    public async Task Execute_ShouldThrowEmailAlreadyInUseException_WhenEmailAlreadyExists()
    {
        CreateUserDTO createUserDTO = new("Test User", "test@example.com", "password123");
        User existingUser = new("1", "Existing User", "test@example.com", true);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(createUserDTO.Email))
            .ReturnsAsync(existingUser);

        var exception = await Assert.ThrowsAsync<EmailAlreadyInUseException>(() => this._sut.Execute(createUserDTO));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Equal("user:email-already-in-use", exception.ErrorCode);
        Assert.Equal("Email already in use", exception.Title);
        Assert.Equal("The email address is already associated with another account.", exception.Detail);

        this._userRepositoryMock.Verify(repository => repository.Insert(It.IsAny<User>()), Times.Never);
    }
}
