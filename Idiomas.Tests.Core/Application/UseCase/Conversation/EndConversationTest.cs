using Idiomas.Core.Application.Exceptions.Conversation;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using Moq;
using System.Net;

using CoreConversation = Idiomas.Core.Domain.Entity.Conversation;

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
    public async Task Execute_ShouldThrowConversationNotFoundException_WhenConversationNotFound()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync((CoreConversation?)null);

        ConversationNotFoundException exception = await Assert.ThrowsAsync<ConversationNotFoundException>(() => _sut.Execute(conversationId, userId));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("conversation:not-found", exception.ErrorCode);
        Assert.Equal("Conversation not found", exception.Title);
        Assert.Equal("The requested conversation was not found.", exception.Detail);
    }

    [Fact]
    public async Task Execute_ShouldThrowConversationAccessDeniedException_WhenUserNotOwner()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        string ownerId = UUIDGenerator.Generate();

        CoreConversation conversation = new(conversationId, ownerId, Language.English, ConversationMode.Free);

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync(conversation);

        ConversationAccessDeniedException exception = await Assert.ThrowsAsync<ConversationAccessDeniedException>(() => _sut.Execute(conversationId, userId));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        Assert.Equal("conversation:access-denied", exception.ErrorCode);
        Assert.Equal("Conversation access denied", exception.Title);
        Assert.Equal("You do not have permission to access this conversation.", exception.Detail);
    }
}
