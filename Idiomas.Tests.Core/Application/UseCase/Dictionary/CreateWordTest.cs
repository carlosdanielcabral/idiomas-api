using System.Net;
using Idiomas.Core.Application.DTO.Dictionary;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.DictionaryCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.Dictionary;

public class CreateWordTest
{
    private readonly Mock<IDictionaryRepository> _dictionaryRepositoryMock;
    private readonly CreateWord _sut;

    public CreateWordTest()
    {
        this._dictionaryRepositoryMock = new Mock<IDictionaryRepository>();
        this._sut = new CreateWord(_dictionaryRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldCreateWord_WhenWordIsNew()
    {
        CreateWordDTO createWordDTO = new("new word", "ipa", new List<CreateMeaningDTO>());

        string userId = "user-id-123";

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetByWord(createWordDTO.Word, userId))
            .ReturnsAsync((Word) null!);

        this._dictionaryRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<Word>()))
            .ReturnsAsync((Word word) => word);

        var result = await _sut.Execute(createWordDTO, userId);

        Assert.NotNull(result);
        Assert.Equal(createWordDTO.Word, result.Name);

        this._dictionaryRepositoryMock.Verify(repository => repository.Insert(It.IsAny<Word>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowApiException_WhenWordAlreadyExists()
    {
        string userId = "user-id-123";

        CreateWordDTO createWordDTO = new("existing word", "ipa", new List<CreateMeaningDTO>());
        Word existingWord = new("word-id", "existing word", "ipa", userId, new List<Meaning>());

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetByWord(createWordDTO.Word, userId))
            .ReturnsAsync(existingWord);

        var exception = await Assert.ThrowsAsync<ApiException>(() => _sut.Execute(createWordDTO, userId));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Equal("Palavra já cadastrada", exception.Message);

        this._dictionaryRepositoryMock.Verify(repository => repository.Insert(It.IsAny<Word>()), Times.Never);
    }
}