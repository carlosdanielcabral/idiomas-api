using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Moq;
using System.Net;

namespace Idiomas.Tests.Core.Application.UseCase.AuthCase;

public class ResetPasswordTest
{
    private readonly Mock<IPasswordResetTokenRepository> _tokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserCredentialRepository> _userCredentialRepositoryMock = new();
    private readonly Mock<IHash> _hashMock = new();

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenTokenDoesNotExist()
    {
        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync((PasswordResetToken?)null);

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("invalid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<ApiException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenTokenIsExpired()
    {
        var token = new PasswordResetToken(Guid.NewGuid(), Guid.NewGuid(), "valid-token", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync(token);

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<ApiException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ThrowsApiExceptionWhenTokenIsAlreadyUsed()
    {
        var token = new PasswordResetToken(Guid.NewGuid(), Guid.NewGuid(), "valid-token", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddMinutes(-5));

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync(token);

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<ApiException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ThrowsApiExceptionWithGenericMessageWhenUserHasNoLocalCredential()
    {
        Guid userId = Guid.NewGuid();
        var token = new PasswordResetToken(Guid.NewGuid(), userId, "valid-token", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new User(userId.ToString(), "João", "joao@example.com");

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync(token);

        this._userRepositoryMock
            .Setup(repository => repository.GetById(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(It.IsAny<string>(), AuthProvider.Local))
            .ReturnsAsync((UserCredential?)null);

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<ApiException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Token inválido ou expirado", exception.Message);
    }

    [Fact]
    public async Task Execute_UpdatesPasswordAndMarksTokenAsUsedWhenValid()
    {
        Guid userId = Guid.NewGuid();
        var token = new PasswordResetToken(Guid.NewGuid(), userId, "valid-token", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new User(userId.ToString(), "João", "joao@example.com");
        var credential = new UserCredential(Guid.NewGuid().ToString(), userId.ToString(), AuthProvider.Local, "oldhash", null);

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByToken(It.IsAny<string>()))
            .ReturnsAsync(token);

        this._userRepositoryMock
            .Setup(repository => repository.GetById(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(It.IsAny<string>(), AuthProvider.Local))
            .ReturnsAsync(credential);

        this._hashMock
            .Setup(hash => hash.Hash(It.IsAny<string>()))
            .Returns("newhash");

        var useCase = new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._hashMock.Object
        );

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        await useCase.Execute(dto);

        Assert.Equal("newhash", credential.PasswordHash);

        this._userCredentialRepositoryMock.Verify(repository => repository.Update(It.Is<UserCredential>(updatedCredential => updatedCredential.PasswordHash == "newhash")), Times.Once);
        this._tokenRepositoryMock.Verify(repository => repository.MarkAsUsed(token), Times.Once);
    }
}
