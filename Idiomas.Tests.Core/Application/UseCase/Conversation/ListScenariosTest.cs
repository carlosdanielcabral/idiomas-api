using Idiomas.Core.Helper.Error;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using CoreScenario = Idiomas.Core.Domain.Entity.Scenario;
using Idiomas.Core.Interface.Repository;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.Conversation;

public class ListScenariosTest
{
    private readonly Mock<IScenarioRepository> _scenarioRepositoryMock;
    private readonly ListScenarios _sut;

    public ListScenariosTest()
    {
        this._scenarioRepositoryMock = new Mock<IScenarioRepository>();
        this._sut = new ListScenarios(_scenarioRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldReturnScenarios_ForLanguage()
    {
        string language = "English";
        Language? parsedLanguage = Language.English;
        List<CoreScenario> expectedScenarios = new()
        {
            new CoreScenario(UUIDGenerator.Generate(), Language.English, "Restaurant", "Ordering food"),
            new CoreScenario(UUIDGenerator.Generate(), Language.English, "Hotel", "Checking in"),
            new CoreScenario(UUIDGenerator.Generate(), Language.English, "Interview", "Job interview")
        };

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetByLanguage(parsedLanguage))
            .ReturnsAsync(expectedScenarios);

        IEnumerable<CoreScenario> result = await _sut.Execute(language);

        Assert.Equal(3, result.Count());
        this._scenarioRepositoryMock.Verify(repository => repository.GetByLanguage(parsedLanguage), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenNoScenarios()
    {
        string language = "German";
        Language? parsedLanguage = Language.German;

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetByLanguage(parsedLanguage))
            .ReturnsAsync(new List<CoreScenario>());

        IEnumerable<CoreScenario> result = await _sut.Execute(language);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnAllScenarios_WhenLanguageIsNull()
    {
        string? language = null;
        Language? parsedLanguage = null;
        List<CoreScenario> expectedScenarios = new()
        {
            new CoreScenario(UUIDGenerator.Generate(), Language.English, "Restaurant", "Ordering food"),
            new CoreScenario(UUIDGenerator.Generate(), Language.Spanish, "Restaurante", "Ordering in Spanish"),
            new CoreScenario(UUIDGenerator.Generate(), Language.French, "Restaurant", "Ordering in French")
        };

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetByLanguage(parsedLanguage))
            .ReturnsAsync(expectedScenarios);

        IEnumerable<CoreScenario> result = await _sut.Execute(language);

        Assert.Equal(3, result.Count());
        this._scenarioRepositoryMock.Verify(repository => repository.GetByLanguage(parsedLanguage), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowLanguageInvalidException_WhenInvalidLanguage()
    {
        string language = "PT";

        LanguageInvalidException exception = await Assert.ThrowsAsync<LanguageInvalidException>(() => _sut.Execute(language));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("common:language-invalid", exception.ErrorCode);
        Assert.Equal("Language invalid", exception.Title);
        Assert.Contains("PT", exception.Detail);
        Assert.Contains("English", exception.Detail);
        Assert.Contains("Spanish", exception.Detail);
        Assert.Contains("French", exception.Detail);
        Assert.Contains("German", exception.Detail);
        Assert.Contains("Italian", exception.Detail);
        Assert.Contains("Portuguese", exception.Detail);
    }

    [Fact]
    public async Task Execute_ShouldParseLanguage_CaseInsensitive()
    {
        string language = "english";
        Language? parsedLanguage = Language.English;
        List<CoreScenario> expectedScenarios = new()
        {
            new CoreScenario(UUIDGenerator.Generate(), Language.English, "Restaurant", "Ordering food")
        };

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetByLanguage(parsedLanguage))
            .ReturnsAsync(expectedScenarios);

        IEnumerable<CoreScenario> result = await _sut.Execute(language);

        Assert.Single(result);
        this._scenarioRepositoryMock.Verify(repository => repository.GetByLanguage(parsedLanguage), Times.Once);
    }
}
