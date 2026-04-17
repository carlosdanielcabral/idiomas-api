using Idiomas.Core.Application.UseCase.ConversationCase;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using Moq;

using CoreConversation = Idiomas.Core.Domain.Entity.Conversation;

namespace Idiomas.Tests.Core.Application.UseCase.Conversation;

public class ListConversationsTest
{
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly ListConversations _sut;

    public ListConversationsTest()
    {
        this._conversationRepositoryMock = new Mock<IConversationRepository>();
        this._sut = new ListConversations(_conversationRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldReturnUserConversations()
    {
        string userId = UUIDGenerator.Generate();
        List<CoreConversation> expectedConversations = new()
        {
            new CoreConversation(UUIDGenerator.Generate(), userId, Language.English, ConversationMode.Free),
            new CoreConversation(UUIDGenerator.Generate(), userId, Language.Spanish, ConversationMode.Guided),
            new CoreConversation(UUIDGenerator.Generate(), userId, Language.French, ConversationMode.Free)
        };

        this._conversationRepositoryMock
            .Setup(repository => repository.GetByUserId(userId))
            .ReturnsAsync(expectedConversations);

        IEnumerable<CoreConversation> result = await _sut.Execute(userId);

        Assert.Equal(3, result.Count());
        this._conversationRepositoryMock.Verify(repository => repository.GetByUserId(userId), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenNoConversations()
    {
        string userId = UUIDGenerator.Generate();

        this._conversationRepositoryMock
            .Setup(repository => repository.GetByUserId(userId))
            .ReturnsAsync(new List<CoreConversation>());

        IEnumerable<CoreConversation> result = await _sut.Execute(userId);

        Assert.Empty(result);
    }
}
