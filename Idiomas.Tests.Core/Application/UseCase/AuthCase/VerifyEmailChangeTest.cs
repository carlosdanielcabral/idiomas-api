using System.Net;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.AuthCase;

public class VerifyEmailChangeTest
{
    private readonly Mock<IEmailChangeRequestRepository> _requestRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ITokenHasher> _tokenHasherMock = new();
    private readonly VerifyEmailChange _sut;

    public VerifyEmailChangeTest()
    {
        this._sut = new VerifyEmailChange(
            this._requestRepositoryMock.Object,
            this._userRepositoryMock.Object,
            this._tokenHasherMock.Object
        );
    }

    [Fact]
    public async Task Execute_UpdatesUserEmailAndMarksRequestAsUsed_WhenTokenIsValid()
    {
        Guid userId = Guid.NewGuid();
        string newEmail = "new@example.com";
        string rawToken = "valid-token";
        string tokenHash = "hashed-token";
        var request = new EmailChangeRequest(Guid.NewGuid(), userId, newEmail, tokenHash, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var user = new User(userId.ToString(), "João", "old@example.com", true);

        this._tokenHasherMock.Setup(hasher => hasher.Hash(rawToken)).Returns(tokenHash);
        this._requestRepositoryMock.Setup(repository => repository.GetByTokenHash(tokenHash)).ReturnsAsync(request);
        this._userRepositoryMock.Setup(repository => repository.GetById(userId.ToString())).ReturnsAsync(user);
        this._userRepositoryMock.Setup(repository => repository.Update(It.IsAny<User>())).ReturnsAsync((User updatedUser) => updatedUser);

        await this._sut.Execute(rawToken);

        this._userRepositoryMock.Verify(repository => repository.Update(It.Is<User>(updatedUser => updatedUser.Email == newEmail)), Times.Once);
        this._requestRepositoryMock.Verify(repository => repository.MarkAsUsed(request), Times.Once);
    }

    [Fact]
    public async Task Execute_ThrowsApiException_WhenTokenDoesNotExist()
    {
        string rawToken = "invalid-token";
        string tokenHash = "hashed-token";

        this._tokenHasherMock.Setup(hasher => hasher.Hash(rawToken)).Returns(tokenHash);
        this._requestRepositoryMock.Setup(repository => repository.GetByTokenHash(tokenHash)).ReturnsAsync((EmailChangeRequest?)null);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(rawToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ThrowsApiException_WhenTokenIsExpired()
    {
        Guid userId = Guid.NewGuid();
        string rawToken = "valid-token";
        string tokenHash = "hashed-token";
        var request = new EmailChangeRequest(Guid.NewGuid(), userId, "new@example.com", tokenHash, DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));

        this._tokenHasherMock.Setup(hasher => hasher.Hash(rawToken)).Returns(tokenHash);
        this._requestRepositoryMock.Setup(repository => repository.GetByTokenHash(tokenHash)).ReturnsAsync(request);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(rawToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ThrowsApiException_WhenTokenIsAlreadyUsed()
    {
        Guid userId = Guid.NewGuid();
        string rawToken = "valid-token";
        string tokenHash = "hashed-token";
        var request = new EmailChangeRequest(Guid.NewGuid(), userId, "new@example.com", tokenHash, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddMinutes(-5));

        this._tokenHasherMock.Setup(hasher => hasher.Hash(rawToken)).Returns(tokenHash);
        this._requestRepositoryMock.Setup(repository => repository.GetByTokenHash(tokenHash)).ReturnsAsync(request);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(rawToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }
}
