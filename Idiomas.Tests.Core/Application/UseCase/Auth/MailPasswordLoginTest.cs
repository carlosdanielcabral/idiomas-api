using System.Net;
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
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
        User user = new("1", "Test User", "test@example.com");
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
    public async Task Execute_ShouldThrowApiException_WhenEmailDoesNotExist()
    {
        MailPasswordLoginDTO loginDto = new("wrong@example.com", "password123");

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(loginDto.Email))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(loginDto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Email ou senha inválidos", exception.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrowApiException_WhenPasswordIsInvalid()
    {
        MailPasswordLoginDTO loginDto = new("test@example.com", "wrongpassword");
        User user = new("1", "Test User", "test@example.com");
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

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(loginDto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Email ou senha inválidos", exception.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrowApiException_WhenUserHasNoLocalCredential()
    {
        MailPasswordLoginDTO loginDto = new("test@example.com", "password123");
        User user = new("1", "Test User", "test@example.com");

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(loginDto.Email))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(user.Id, AuthProvider.Local))
            .ReturnsAsync((UserCredential?)null);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(loginDto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Email ou senha inválidos", exception.Message);
    }
}
