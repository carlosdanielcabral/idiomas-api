using System.Net;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Exceptions.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.Auth;

public class MailPasswordLoginTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserCredentialRepository> _userCredentialRepositoryMock;
    private readonly Mock<IHash> _hashMock;
    private readonly MailPasswordLogin _sut;

    public MailPasswordLoginTest()
    {
        this._userRepositoryMock = new Mock<IUserRepository>();
        this._userCredentialRepositoryMock = new Mock<IUserCredentialRepository>();
        this._hashMock = new Mock<IHash>();
        this._sut = new MailPasswordLogin(
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._hashMock.Object
        );
    }

    [Fact]
    public async Task Execute_ShouldReturnUser_WhenCredentialsAreValid()
    {
        MailPasswordLoginDTO loginDto = new("test@example.com", "password123");
        User user = new("1", "Test User", "test@example.com", true);
        UserCredential credential = new("cred-1", "1", AuthProvider.Local, "hashed_password", null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(loginDto.Email))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync(credential);

        this._hashMock
            .Setup(hash => hash.Verify(loginDto.Password, credential.PasswordHash!))
            .Returns(true);

        var result = await this._sut.Execute(loginDto);

        Assert.NotNull(result);
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidCredentialsException_WhenEmailDoesNotExist()
    {
        MailPasswordLoginDTO loginDto = new("wrong@example.com", "password123");

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(loginDto.Email))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(() => this._sut.Execute(loginDto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:invalid-credentials", exception.ErrorCode);
        Assert.Equal("Invalid credentials", exception.Title);
        Assert.Equal("The email or password is invalid.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidCredentialsException_WhenPasswordIsInvalid()
    {
        MailPasswordLoginDTO loginDto = new("test@example.com", "wrongpassword");
        User user = new("1", "Test User", "test@example.com", true);
        UserCredential credential = new("cred-1", "1", AuthProvider.Local, "hashed_password", null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(loginDto.Email))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync(credential);

        this._hashMock
            .Setup(hash => hash.Verify(loginDto.Password, credential.PasswordHash!))
            .Returns(false);

        var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(() => this._sut.Execute(loginDto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:invalid-credentials", exception.ErrorCode);
        Assert.Equal("Invalid credentials", exception.Title);
        Assert.Equal("The email or password is invalid.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidCredentialsException_WhenUserHasNoLocalCredential()
    {
        MailPasswordLoginDTO loginDto = new("test@example.com", "password123");
        User user = new("1", "Test User", "test@example.com", true);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(loginDto.Email))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync((UserCredential?)null);

        var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(() => this._sut.Execute(loginDto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:invalid-credentials", exception.ErrorCode);
        Assert.Equal("Invalid credentials", exception.Title);
        Assert.Equal("The email or password is invalid.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ShouldThrowEmailNotVerifiedException_WhenEmailIsNotVerified()
    {
        MailPasswordLoginDTO loginDto = new("test@example.com", "password123");
        User user = new("1", "Test User", "test@example.com", false);
        UserCredential credential = new("cred-1", "1", AuthProvider.Local, "hashed_password", null);

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(loginDto.Email))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync(credential);

        this._hashMock
            .Setup(hash => hash.Verify(loginDto.Password, credential.PasswordHash!))
            .Returns(true);

        var exception = await Assert.ThrowsAsync<EmailNotVerifiedException>(() => this._sut.Execute(loginDto));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        Assert.Equal("auth:email-not-verified", exception.ErrorCode);
        Assert.Equal("Email not verified", exception.Title);
        Assert.Equal("The email has not been verified. Check your inbox to activate your account.", exception.Detail);
    }
}
