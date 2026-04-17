using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;
using Idiomas.Core.Interface.Repository;
using Moq;

namespace Idiomas.Tests.Core.Interface.Repository;

public class ConversationRepositoryInterfaceTest
{
    [Fact]
    public async Task IConversationRepository_ShouldBeMockable()
    {
        Mock<IConversationRepository> mockRepository = new();
        string conversationId = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        Conversation expectedConversation = new(conversationId, userId, Language.English, ConversationMode.Free);

        mockRepository
            .Setup(repository => repository.GetById(conversationId))
            .ReturnsAsync(expectedConversation);

        IConversationRepository repository = mockRepository.Object;
        Conversation? result = await repository.GetById(conversationId);

        Assert.NotNull(result);
        Assert.Equal(expectedConversation.Id, result.Id);
    }

    [Fact]
    public async Task IScenarioRepository_ShouldBeMockable()
    {
        Mock<IScenarioRepository> mockRepository = new();
        List<Scenario> expectedScenarios = new()
        {
            new Scenario(UUIDGenerator.Generate(), Language.English, "Restaurant", "Ordering food")
        };

        mockRepository
            .Setup(repository => repository.GetByLanguage(Language.English))
            .ReturnsAsync(expectedScenarios);

        IScenarioRepository repository = mockRepository.Object;
        IEnumerable<Scenario> result = await repository.GetByLanguage(Language.English);

        Assert.Single(result);
    }
}
