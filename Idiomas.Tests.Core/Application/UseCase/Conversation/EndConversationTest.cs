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

public class EndConversationTest
{
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly EndConversation _sut;

    public EndConversationTest()
    {
        this._conversationRepositoryMock = new Mock<IConversationRepository>();
        this._sut = new EndConversation(_conversationRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldEndConversation()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();

        CoreConversation conversation = new(conversationId, userId, Language.English, ConversationMode.Free);

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync(conversation);

        this._conversationRepositoryMock
            .Setup(repository => repository.Inactivate(conversationId))
            .Returns(Task.CompletedTask);

        await _sut.Execute(conversationId, userId);

        this._conversationRepositoryMock.Verify(repository => repository.Inactivate(conversationId), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenConversationNotFound()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync((CoreConversation?)null);

        ApiException exception = await Assert.ThrowsAsync<ApiException>(() => _sut.Execute(conversationId, userId));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenUserNotOwner()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        string ownerId = UUIDGenerator.Generate();

        CoreConversation conversation = new(conversationId, ownerId, Language.English, ConversationMode.Free);

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync(conversation);

        ApiException exception = await Assert.ThrowsAsync<ApiException>(() => _sut.Execute(conversationId, userId));
        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
    }
}
