using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;

namespace Idiomas.Tests.Core.Domain.Entity;

public class ConversationEntityTest
{
    [Fact]
    public void Conversation_ShouldInitialize_WithCorrectValues()
    {
        string id = UUIDGenerator.Generate();
        string userId = UUIDGenerator.Generate();
        Language language = Language.English;
        ConversationMode mode = ConversationMode.Free;

        Conversation conversation = new(id, userId, language, mode);

        Assert.Equal(id, conversation.Id);
        Assert.Equal(userId, conversation.UserId);
        Assert.Equal(language, conversation.Language);
        Assert.Equal(mode, conversation.Mode);
        Assert.True(conversation.IsActive);
        Assert.NotNull(conversation.Messages);
        Assert.Empty(conversation.Messages);
    }

    [Fact]
    public void Message_ShouldInitialize_WithCorrectValues()
    {
        string id = UUIDGenerator.Generate();
        string conversationId = UUIDGenerator.Generate();
        MessageRole role = MessageRole.User;
        string content = "Hello, how are you?";

        Message message = new(id, conversationId, role, content);

        Assert.Equal(id, message.Id);
        Assert.Equal(conversationId, message.ConversationId);
        Assert.Equal(role, message.Role);
        Assert.Equal(content, message.Content);
        Assert.NotNull(message.Corrections);
        Assert.Empty(message.Corrections);
    }

    [Fact]
    public void Correction_ShouldInitialize_WithCorrectValues()
    {
        string id = UUIDGenerator.Generate();
        string messageId = UUIDGenerator.Generate();
        string originalFragment = "I go yesterday";
        string suggestedFragment = "I went yesterday";
        string explanation = "Use past tense for completed actions";
        ErrorType type = ErrorType.Grammar;

        Correction correction = new(id, messageId, originalFragment, suggestedFragment, explanation, type);

        Assert.Equal(id, correction.Id);
        Assert.Equal(messageId, correction.MessageId);
        Assert.Equal(originalFragment, correction.OriginalFragment);
        Assert.Equal(suggestedFragment, correction.SuggestedFragment);
        Assert.Equal(explanation, correction.Explanation);
        Assert.Equal(type, correction.Type);
    }

    [Fact]
    public void Scenario_ShouldInitialize_WithCorrectValues()
    {
        string id = UUIDGenerator.Generate();
        Language language = Language.English;
        string title = "At the Restaurant";
        string description = "Ordering food and drinks";

        Scenario scenario = new(id, language, title, description);

        Assert.Equal(id, scenario.Id);
        Assert.Equal(language, scenario.Language);
        Assert.Equal(title, scenario.Title);
        Assert.Equal(description, scenario.Description);
        Assert.True(scenario.IsActive);
    }
}
