using System.Net;
using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.DictionaryCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.Dictionary;

public class UpdateWordTest
{
    private readonly Mock<IDictionaryRepository> _dictionaryRepositoryMock;
    private readonly UpdateWord _sut;

    public UpdateWordTest()
    {
        this._dictionaryRepositoryMock = new Mock<IDictionaryRepository>();
        this._sut = new UpdateWord(_dictionaryRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldUpdateWord_WhenDataIsValid()
    {
        string userId = "user-id-123";
        string wordId = "word-id-1";

        UpdateWordDTO updateDto = new("updated word", "ipa-updated", new List<CreateMeaningDTO>());
        Word existingWord = new(wordId, "original word", "ipa-original", userId, new List<Meaning>());

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetById(wordId))
            .ReturnsAsync(existingWord);

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetByWord(updateDto.Word, userId))
            .ReturnsAsync((Word) null!);

        this._dictionaryRepositoryMock
            .Setup(repository => repository.Update(It.IsAny<Word>()))
            .ReturnsAsync((Word word) => word);

        var result = await this._sut.Execute(wordId, updateDto, userId);

        Assert.NotNull(result);
        Assert.Equal(updateDto.Word, result.Name);
        Assert.Equal(updateDto.Ipa, result.Ipa);

        this._dictionaryRepositoryMock.Verify(repository => repository.Update(It.IsAny<Word>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowNotFound_WhenWordDoesNotExist()
    {
        string userId = "user-id-123";
        string wordId = "non-existing-word";

        UpdateWordDTO updateDto = new("any word", "any-ipa", new List<CreateMeaningDTO>());

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetById(wordId))
            .ReturnsAsync((Word) null!);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(wordId, updateDto, userId));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Palavra não encontrada", exception.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrowConflict_WhenNewWordNameAlreadyExists()
    {
        string userId = "user-id-123";
        string wordId = "word-id-1";

        UpdateWordDTO updateDto = new("existing-word-name", "ipa-updated", new List<CreateMeaningDTO>());
        Word originalWord = new(wordId, "original word", "ipa-original", userId, new List<Meaning>());
        Word conflictingWord = new("word-id-2", "existing-word-name", "ipa", userId, new List<Meaning>());

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetById(wordId))
            .ReturnsAsync(originalWord);

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetByWord(updateDto.Word, userId))
            .ReturnsAsync(conflictingWord);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(wordId, updateDto, userId));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Equal("Palavra já cadastrada", exception.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrowForbidden_WhenUserIsNotTheOwner()
    {
        string ownerId = "owner-id-123";
        string attackerId = "attacker-id-456";
        string wordId = "word-id-1";

        UpdateWordDTO updateDto = new("updated word", "ipa-updated", new List<CreateMeaningDTO>());
        Word existingWord = new(wordId, "original word", "ipa-original", ownerId, new List<Meaning>());

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetById(wordId))
            .ReturnsAsync(existingWord);

        var exception = await Assert.ThrowsAsync<ApiException>(() => this._sut.Execute(wordId, updateDto, attackerId));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        Assert.Equal("Você não tem permissão para atualizar esta palavra", exception.Message);
    }

    [Fact]
    public async Task Execute_ShouldUpdate_WhenWordNameIsUnchanged()
    {
        string userId = "user-id-123";
        string wordId = "word-id-1";

        Word existingWord = new(wordId, "same-word", "ipa-original", userId, new List<Meaning>());
        UpdateWordDTO updateDto = new("same-word", "ipa-updated", new List<CreateMeaningDTO> { new("new meaning", "example") });

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetById(wordId))
            .ReturnsAsync(existingWord);

        this._dictionaryRepositoryMock
            .Setup(repository => repository.Update(It.Is<Word>(word => word.Id == wordId && word.Ipa == "ipa-updated")))
            .ReturnsAsync((Word w) => w);

        var result = await this._sut.Execute(wordId, updateDto, userId);

        Assert.NotNull(result);
        Assert.Equal("ipa-updated", result.Ipa);
        Assert.Equal("same-word", result.Name);

        this._dictionaryRepositoryMock.Verify(repository => repository.GetByWord(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        this._dictionaryRepositoryMock.Verify(repository => repository.Update(It.IsAny<Word>()), Times.Once);
    }
}