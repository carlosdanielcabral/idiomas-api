using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error.Auth;
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
    private readonly Mock<ITokenHasher> _tokenHasherMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public ResetPasswordTest()
    {
        this._tokenHasherMock.Setup(hasher => hasher.Hash(It.IsAny<string>())).Returns("hashed-token");

        this._unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.ExecuteAsync(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> operation) => operation());
    }

    private ResetPassword CreateSut()
    {
        return new ResetPassword(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._userCredentialRepositoryMock.Object,
            this._hashMock.Object,
            this._tokenHasherMock.Object,
            this._unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Execute_ThrowsTokenInvalidOrExpiredExceptionWhenTokenDoesNotExist()
    {
        this._tokenRepositoryMock
            .Setup(repository => repository.GetByTokenHash(It.IsAny<string>()))
            .ReturnsAsync((PasswordResetToken?)null);

        var useCase = this.CreateSut();

        var dto = new ResetPasswordDTO("invalid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<TokenInvalidOrExpiredException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:token-invalid-or-expired", exception.ErrorCode);
        Assert.Equal("Token invalid or expired", exception.Title);
        Assert.Equal("The token is invalid or has expired.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ThrowsTokenInvalidOrExpiredExceptionWhenTokenIsExpired()
    {
        var token = new PasswordResetToken(Guid.NewGuid(), Guid.NewGuid(), "valid-hash", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByTokenHash(It.IsAny<string>()))
            .ReturnsAsync(token);

        var useCase = this.CreateSut();

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<TokenInvalidOrExpiredException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:token-invalid-or-expired", exception.ErrorCode);
        Assert.Equal("Token invalid or expired", exception.Title);
        Assert.Equal("The token is invalid or has expired.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ThrowsTokenInvalidOrExpiredExceptionWhenTokenIsAlreadyUsed()
    {
        var token = new PasswordResetToken(Guid.NewGuid(), Guid.NewGuid(), "valid-hash", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddMinutes(-5));

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByTokenHash(It.IsAny<string>()))
            .ReturnsAsync(token);

        var useCase = this.CreateSut();

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<TokenInvalidOrExpiredException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:token-invalid-or-expired", exception.ErrorCode);
        Assert.Equal("Token invalid or expired", exception.Title);
        Assert.Equal("The token is invalid or has expired.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ThrowsTokenInvalidOrExpiredExceptionWhenUserHasNoLocalCredential()
    {
        Guid userId = Guid.NewGuid();
        var token = new PasswordResetToken(Guid.NewGuid(), userId, "valid-hash", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new User(userId.ToString(), "João", "joao@example.com", true);

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByTokenHash(It.IsAny<string>()))
            .ReturnsAsync(token);

        this._userRepositoryMock
            .Setup(repository => repository.GetById(It.IsAny<string>()))
            .ReturnsAsync(user);

        this._userCredentialRepositoryMock
            .Setup(repository => repository.GetByUserIdAndProvider(It.IsAny<string>(), AuthProvider.Local))
            .ReturnsAsync((UserCredential?)null);

        var useCase = this.CreateSut();

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        var exception = await Assert.ThrowsAsync<TokenInvalidOrExpiredException>(() => useCase.Execute(dto));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:token-invalid-or-expired", exception.ErrorCode);
        Assert.Equal("Token invalid or expired", exception.Title);
        Assert.Equal("The token is invalid or has expired.", exception.Detail);
    }

    [Fact]
    public async Task Execute_UpdatesPasswordAndMarksTokenAsUsedWhenValid()
    {
        Guid userId = Guid.NewGuid();
        var token = new PasswordResetToken(Guid.NewGuid(), userId, "valid-hash", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new User(userId.ToString(), "João", "joao@example.com", true);
        var credential = new UserCredential(Guid.NewGuid().ToString(), userId.ToString(), AuthProvider.Local, "oldhash", null);

        this._tokenRepositoryMock
            .Setup(repository => repository.GetByTokenHash(It.IsAny<string>()))
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

        var useCase = this.CreateSut();

        var dto = new ResetPasswordDTO("valid-token", "newpassword123");

        await useCase.Execute(dto);

        Assert.Equal("newhash", credential.PasswordHash);

        this._userCredentialRepositoryMock.Verify(repository => repository.Update(It.Is<UserCredential>(updatedCredential => updatedCredential.PasswordHash == "newhash")), Times.Once);
        this._tokenRepositoryMock.Verify(repository => repository.MarkAsUsed(token), Times.Once);
    }
}
