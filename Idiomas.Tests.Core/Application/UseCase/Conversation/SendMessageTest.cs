using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Moq;
using System.Net;

using CoreConversation = Idiomas.Core.Domain.Entity.Conversation;
using CoreMessage = Idiomas.Core.Domain.Entity.Message;
using CoreCorrection = Idiomas.Core.Domain.Entity.Correction;
using CoreScenario = Idiomas.Core.Domain.Entity.Scenario;

namespace Idiomas.Tests.Core.Application.UseCase.Conversation;

public class SendMessageTest
{
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly Mock<IScenarioRepository> _scenarioRepositoryMock;
    private readonly Mock<IConversationLLMService> _llmServiceMock;
    private readonly SendMessage _sut;

    public SendMessageTest()
    {
        this._conversationRepositoryMock = new Mock<IConversationRepository>();
        this._scenarioRepositoryMock = new Mock<IScenarioRepository>();
        this._llmServiceMock = new Mock<IConversationLLMService>();
        this._sut = new SendMessage(
            _conversationRepositoryMock.Object,
            _scenarioRepositoryMock.Object,
            _llmServiceMock.Object
        );
    }

    [Fact]
    public async Task Execute_ShouldSendMessageAndReturnResponse()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        string content = "Hello, how are you?";

        CoreConversation conversation = new(conversationId, userId, Language.English, ConversationMode.Free);
        SendMessageRequest request = new(content);

        ConversationLLMResponse llmResponse = new(
            "I'm doing well, thank you! How about you?", 
            new List<CorrectionResponse>());

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync((CoreConversation?)conversation);

        this._llmServiceMock
            .Setup(service => service.SendMessageAsync(conversation, content, null))
            .ReturnsAsync(llmResponse);

        this._conversationRepositoryMock
            .Setup(repository => repository.InsertMessage(It.IsAny<CoreMessage>()))
            .ReturnsAsync((CoreMessage message) => message);

        MessageResponse result = await _sut.Execute(conversationId, request, userId);

        Assert.NotNull(result);
        Assert.Equal(llmResponse.Content, result.Content);
        Assert.Equal(MessageRole.Assistant, result.Role);

        this._conversationRepositoryMock.Verify(repository => repository.InsertMessage(It.Is<CoreMessage>(m => m.Role == MessageRole.User)), Times.Once);
        this._conversationRepositoryMock.Verify(repository => repository.InsertMessage(It.Is<CoreMessage>(m => m.Role == MessageRole.Assistant)), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldReturnCorrections_WhenLLMReturnsCorrections()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        string content = "I go yesterday";

        CoreConversation conversation = new(conversationId, userId, Language.English, ConversationMode.Free);
        SendMessageRequest request = new(content);

        ConversationLLMResponse llmResponse = new(
            "Oh, what did you do yesterday?",
            new List<CorrectionResponse>
            {
                new("I go yesterday", "I went yesterday", "Use past tense for completed actions", ErrorType.Grammar)
            });

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync((CoreConversation?)conversation);

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync(conversation);

        this._llmServiceMock
            .Setup(service => service.SendMessageAsync(conversation, content, null))
            .ReturnsAsync(llmResponse);

        this._conversationRepositoryMock
            .Setup(repository => repository.InsertMessage(It.IsAny<CoreMessage>()))
            .ReturnsAsync((CoreMessage message) => message);

        this._conversationRepositoryMock
            .Setup(repository => repository.InsertCorrection(It.IsAny<CoreCorrection>()))
            .ReturnsAsync((CoreCorrection correction) => correction);

        MessageResponse result = await _sut.Execute(conversationId, request, userId);

        Assert.Single(result.Corrections);
        Assert.Equal("I go yesterday", result.Corrections[0].OriginalFragment);
        Assert.Equal("I went yesterday", result.Corrections[0].SuggestedFragment);
    }

    [Fact]
    public async Task Execute_ShouldUseScenarioDescription_WhenScenarioExists()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        string scenarioId = UUIDGenerator.Generate();
        string content = "I'd like a table for two";

        CoreConversation conversation = new(conversationId, userId, Language.English, ConversationMode.Guided);
        conversation.SetScenarioId(scenarioId);
        SendMessageRequest request = new(content);

        CoreScenario scenario = new(scenarioId, Language.English, "At the Restaurant", "Ordering food at a restaurant");

        ConversationLLMResponse llmResponse = new("Certainly! Right this way.", new List<CorrectionResponse>());

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync(conversation);

        this._scenarioRepositoryMock
            .Setup(repository => repository.GetById(scenarioId))
            .ReturnsAsync(scenario);

        this._llmServiceMock
            .Setup(service => service.SendMessageAsync(conversation, content, scenario.Description))
            .ReturnsAsync(llmResponse);

        this._conversationRepositoryMock
            .Setup(repository => repository.InsertMessage(It.IsAny<CoreMessage>()))
            .ReturnsAsync((CoreMessage message) => message);

        MessageResponse result = await _sut.Execute(conversationId, request, userId);

        Assert.NotNull(result);
        this._llmServiceMock.Verify(service => service.SendMessageAsync(conversation, content, scenario.Description), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenConversationNotFound()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        SendMessageRequest request = new("Hello");

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync((CoreConversation?)null);

        ApiException exception = await Assert.ThrowsAsync<ApiException>(() => _sut.Execute(conversationId, request, userId));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenConversationNotOwnedByUser()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        string ownerId = UUIDGenerator.Generate();

        CoreConversation conversation = new(conversationId, ownerId, Language.English, ConversationMode.Free);
        SendMessageRequest request = new("Hello");

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync(conversation);

        ApiException exception = await Assert.ThrowsAsync<ApiException>(() => _sut.Execute(conversationId, request, userId));
        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
    }
}
