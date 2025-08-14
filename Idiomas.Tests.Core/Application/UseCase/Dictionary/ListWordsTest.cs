using Idiomas.Core.Application.UseCase.DictionaryCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.Dictionary;

public class ListWordsTest
{
    private readonly Mock<IDictionaryRepository> _dictionaryRepositoryMock;
    private readonly ListWords _sut;

    public ListWordsTest()
    {
        this._dictionaryRepositoryMock = new Mock<IDictionaryRepository>();
        this._sut = new ListWords(_dictionaryRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldReturnOnlyTheCorrectWordsForTheUser()
    {
        string targetUserId = "user-id-123";
        string otherUserId = "user-id-456";

        var allWords = new List<Word>
        {
            new("1", "word1", "ipa1", targetUserId, new List<Meaning>()),
            new("2", "word2", "ipa2", otherUserId, new List<Meaning>()),
            new("3", "word3", "ipa3", targetUserId, new List<Meaning>())
        };

        var expectedWords = allWords.Where(w => w.UserId == targetUserId).ToList();

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetAll(targetUserId))
            .ReturnsAsync(expectedWords);

        var result = await this._sut.Execute(targetUserId);
        var resultList = result.ToList();

        Assert.NotNull(result);
        Assert.Equal(expectedWords.Count, resultList.Count);
        
        for (int i = 0; i < expectedWords.Count; i++)
        {
            Assert.IsType<Word>(resultList[i]);
            Assert.Equal(expectedWords[i].Id, resultList[i].Id);
            Assert.Equal(expectedWords[i].Name, resultList[i].Name);
        }
        
        this._dictionaryRepositoryMock.Verify(repository => repository.GetAll(targetUserId), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenUserHasNoWords()
    {
        string userId = "user-with-no-words";

        this._dictionaryRepositoryMock
            .Setup(repository => repository.GetAll(userId))
            .ReturnsAsync(new List<Word>());

        var result = await _sut.Execute(userId);

        Assert.NotNull(result);
        Assert.Empty(result);

        this._dictionaryRepositoryMock.Verify(repository => repository.GetAll(userId), Times.Once);
    }
}