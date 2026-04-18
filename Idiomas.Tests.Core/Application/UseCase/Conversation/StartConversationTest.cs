using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using Moq;
using System.Net;

using CoreConversation = Idiomas.Core.Domain.Entity.Conversation;
using CoreScenario = Idiomas.Core.Domain.Entity.Scenario;

namespace Idiomas.Tests.Core.Application.UseCase.Conversation;

public class StartConversationTest
{
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly Mock<IScenarioRepository> _scenarioRepositoryMock;
    private readonly StartConversation _sut;

    public StartConversationTest()
    {
        this._conversationRepositoryMock = new Mock<IConversationRepository>();
        this._scenarioRepositoryMock = new Mock<IScenarioRepository>();
        this._sut = new StartConversation(
            _conversationRepositoryMock.Object,
            _scenarioRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldCreateConversation_WhenValidRequest()
    {
        string userId = UUIDGenerator.Generate();
        string conversationId = UUIDGenerator.Generate();
        StartConversationRequest request = new(Language.English, ConversationMode.Free, null);
        CoreConversation expectedConversation = new(conversationId, userId, Language.English, ConversationMode.Free);

        this._conversationRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<CoreConversation>()))
            .ReturnsAsync((CoreConversation conversation) => conversation);

        CoreConversation result = await _sut.Execute(request, userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(Language.English, result.Language);
        Assert.Equal(ConversationMode.Free, result.Mode);
        Assert.Null(result.ScenarioId);
        Assert.True(result.IsActive);

        this._conversationRepositoryMock.Verify(repository => repository.Insert(It.IsAny<CoreConversation>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldCreateGuidedConversation_WithScenarioId()
    {
        string userId = UUIDGenerator.Generate();
        string scenarioId = UUIDGenerator.Generate();
        StartConversationRequest request = new(Language.Spanish, ConversationMode.Guided, scenarioId);

        CoreScenario scenario = new(scenarioId, Language.Spanish, "Hotel Check-in", "Checking into a hotel");

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetById(scenarioId))
            .ReturnsAsync(scenario);

        this._conversationRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<CoreConversation>()))
            .ReturnsAsync((CoreConversation conversation) => conversation);

        CoreConversation result = await _sut.Execute(request, userId);

        Assert.NotNull(result);
        Assert.Equal(ConversationMode.Guided, result.Mode);
        Assert.Equal(scenarioId, result.ScenarioId);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenScenarioNotFound()
    {
        string userId = UUIDGenerator.Generate();
        string scenarioId = UUIDGenerator.Generate();
        StartConversationRequest request = new(Language.English, ConversationMode.Guided, scenarioId);

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetById(scenarioId))
            .ReturnsAsync((CoreScenario?)null);

        ApiException exception = await Assert.ThrowsAsync<ApiException>(() => _sut.Execute(request, userId));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenScenarioLanguageDoesNotMatch()
    {
        string userId = UUIDGenerator.Generate();
        string scenarioId = UUIDGenerator.Generate();
        StartConversationRequest request = new(Language.French, ConversationMode.Guided, scenarioId);

        CoreScenario scenario = new(scenarioId, Language.Spanish, "Hotel", "Hotel scenario in Spanish");

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetById(scenarioId))
            .ReturnsAsync(scenario);

        ApiException exception = await Assert.ThrowsAsync<ApiException>(() => _sut.Execute(request, userId));
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }
}
