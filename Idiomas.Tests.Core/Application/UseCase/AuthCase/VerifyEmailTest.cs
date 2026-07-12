using System.Net;
using Idiomas.Core.Application.Exceptions.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.AuthCase;

public class VerifyEmailTest
{
    private readonly Mock<IEmailVerificationTokenRepository> _tokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ITokenHasher> _tokenHasherMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly VerifyEmail _sut;

    public VerifyEmailTest()
    {
        this._unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.ExecuteAsync(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> operation) => operation());

        this._sut = new VerifyEmail(
            this._tokenRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._tokenHasherMock.Object,
            this._unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Execute_MarksUserAsVerifiedAndInvalidatesToken_WhenTokenIsValid()
    {
        Guid userId = Guid.NewGuid();
        string rawToken = "valid-token";
        string tokenHash = "hashed-token";
        var token = new EmailVerificationToken(Guid.NewGuid(), userId, tokenHash, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new User(userId.ToString(), "João", "joao@example.com", false);

        this._tokenHasherMock.Setup(hasher => hasher.Hash(rawToken)).Returns(tokenHash);
        this._tokenRepositoryMock.Setup(repo => repo.GetByTokenHash(tokenHash)).ReturnsAsync(token);
        this._userRepositoryMock.Setup(repo => repo.GetById(userId.ToString())).ReturnsAsync(user);
        this._userRepositoryMock.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync((User updatedUser) => updatedUser);

        await this._sut.Execute(rawToken);

        Assert.True(user.IsEmailVerified);
        this._userRepositoryMock.Verify(repo => repo.Update(It.Is<User>(updatedUser => updatedUser.IsEmailVerified)), Times.Once);
        this._tokenRepositoryMock.Verify(repo => repo.MarkAsUsed(token), Times.Once);
    }

    [Fact]
    public async Task Execute_ThrowsTokenInvalidOrExpiredException_WhenTokenDoesNotExist()
    {
        string rawToken = "invalid-token";
        string tokenHash = "hashed-token";

        this._tokenHasherMock.Setup(hasher => hasher.Hash(rawToken)).Returns(tokenHash);
        this._tokenRepositoryMock.Setup(repo => repo.GetByTokenHash(tokenHash)).ReturnsAsync((EmailVerificationToken?)null);

        var exception = await Assert.ThrowsAsync<TokenInvalidOrExpiredException>(() => this._sut.Execute(rawToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:token-invalid-or-expired", exception.ErrorCode);
        Assert.Equal("Token invalid or expired", exception.Title);
        Assert.Equal("The token is invalid or has expired.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ThrowsTokenInvalidOrExpiredException_WhenTokenIsExpired()
    {
        Guid userId = Guid.NewGuid();
        string rawToken = "valid-token";
        string tokenHash = "hashed-token";
        var token = new EmailVerificationToken(Guid.NewGuid(), userId, tokenHash, DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));

        this._tokenHasherMock.Setup(hasher => hasher.Hash(rawToken)).Returns(tokenHash);
        this._tokenRepositoryMock.Setup(repo => repo.GetByTokenHash(tokenHash)).ReturnsAsync(token);

        var exception = await Assert.ThrowsAsync<TokenInvalidOrExpiredException>(() => this._sut.Execute(rawToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:token-invalid-or-expired", exception.ErrorCode);
        Assert.Equal("Token invalid or expired", exception.Title);
        Assert.Equal("The token is invalid or has expired.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ThrowsTokenInvalidOrExpiredException_WhenTokenIsAlreadyUsed()
    {
        Guid userId = Guid.NewGuid();
        string rawToken = "valid-token";
        string tokenHash = "hashed-token";
        var token = new EmailVerificationToken(Guid.NewGuid(), userId, tokenHash, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddMinutes(-5));

        this._tokenHasherMock.Setup(hasher => hasher.Hash(rawToken)).Returns(tokenHash);
        this._tokenRepositoryMock.Setup(repo => repo.GetByTokenHash(tokenHash)).ReturnsAsync(token);

        var exception = await Assert.ThrowsAsync<TokenInvalidOrExpiredException>(() => this._sut.Execute(rawToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("auth:token-invalid-or-expired", exception.ErrorCode);
        Assert.Equal("Token invalid or expired", exception.Title);
        Assert.Equal("The token is invalid or has expired.", exception.Detail);
    }
}
