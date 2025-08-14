using System.Net;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.DictionaryCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.Dictionary;

public class DeleteWordTest
{
    private readonly Mock<IDictionaryRepository> _dictionaryRepositoryMock;
    private readonly DeleteWord _sut;

    public DeleteWordTest()
    {
        this._dictionaryRepositoryMock = new Mock<IDictionaryRepository>();
        this._sut = new DeleteWord(_dictionaryRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldDeleteWord_WhenUserIsOwner()
    {
        string userId = "user-id-123";
        string wordId = "word-id-1";

        Word existingWord = new(wordId, "word-to-delete", "ipa", userId, new List<Meaning>());

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetById(wordId))
            .ReturnsAsync(existingWord);

        this._dictionaryRepositoryMock
            .Setup(repository => repository.Delete(wordId))
            .Returns(Task.CompletedTask);

        await this._sut.Execute(wordId, userId);

        this._dictionaryRepositoryMock.Verify(repository => repository.Delete(wordId), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowNotFound_WhenWordDoesNotExist()
    {
        string userId = "user-id-123";
        string wordId = "non-existing-word";

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetById(wordId))
            .ReturnsAsync((Word) null!);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(wordId, userId));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Palavra não encontrada", exception.Message);

        this._dictionaryRepositoryMock.Verify(repository => repository.Delete(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowForbidden_WhenUserIsNotTheOwner()
    {
        string ownerId = "owner-id-123";
        string attackerId = "attacker-id-456";
        string wordId = "word-id-1";

        Word existingWord = new(wordId, "word-to-delete", "ipa", ownerId, new List<Meaning>());

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetById(wordId))
            .ReturnsAsync(existingWord);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(wordId, attackerId));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        Assert.Equal("Você não tem permissão para deletar esta palavra", exception.Message);

        this._dictionaryRepositoryMock.Verify(repository => repository.Delete(It.IsAny<string>()), Times.Never);
    }
}