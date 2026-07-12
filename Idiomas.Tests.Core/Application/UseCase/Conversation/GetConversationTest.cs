using Idiomas.Core.Application.Error.Conversation;
using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using Moq;
using System.Net;

using CoreConversation = Idiomas.Core.Domain.Entity.Conversation;

namespace Idiomas.Tests.Core.Application.UseCase.Conversation;

public class GetConversationTest
{
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly GetConversation _sut;

    public GetConversationTest()
    {
        this._conversationRepositoryMock = new Mock<IConversationRepository>();
        this._sut = new GetConversation(_conversationRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldReturnConversation_WhenExists()
    {
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        string scenarioId = UUIDGenerator.Generate();

        CoreConversation conversation = new(conversationId, userId, Language.Spanish, ConversationMode.Guided, scenarioId);

        this._conversationRepositoryMock
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync(conversation);

        CoreConversation result = await _sut.Execute(conversationId, userId);

        Assert.NotNull(result);
        Assert.Equal(conversationId, result.Id);
        Assert.Equal(userId, result.UserId);
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
