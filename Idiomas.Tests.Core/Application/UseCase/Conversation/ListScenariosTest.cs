using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using CoreConversation = Idiomas.Core.Domain.Entity.Conversation;
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
        Language language = Language.English;
        List<CoreScenario> expectedScenarios = new()
        {
            new CoreScenario(UUIDGenerator.Generate(), Language.English, "Restaurant", "Ordering food"),
            new CoreScenario(UUIDGenerator.Generate(), Language.English, "Hotel", "Checking in"),
            new CoreScenario(UUIDGenerator.Generate(), Language.English, "Interview", "Job interview")
        };

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetByLanguage(language))
            .ReturnsAsync(expectedScenarios);

        IEnumerable<CoreScenario> result = await _sut.Execute(language);

        Assert.Equal(3, result.Count());
        this._scenarioRepositoryMock.Verify(repository => repository.GetByLanguage(language), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenNoScenarios()
    {
        Language language = Language.German;

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetByLanguage(language))
            .ReturnsAsync(new List<CoreScenario>());

        IEnumerable<CoreScenario> result = await _sut.Execute(language);

        Assert.Empty(result);
    }
}
