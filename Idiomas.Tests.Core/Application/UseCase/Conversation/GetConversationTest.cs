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
