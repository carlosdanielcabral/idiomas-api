using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Domain.Enum;

namespace Idiomas.Tests.Core.Application.DTO;

public class ConversationDTOTest
{
    [Fact]
    public void StartConversationRequest_ShouldInitialize_WithCorrectValues()
    {
        Language language = Language.Spanish;
        ConversationMode mode = ConversationMode.Guided;
        string? scenarioId = "scenario-123";

        StartConversationRequest request = new(language, mode, scenarioId);

        Assert.Equal(language, request.Language);
        Assert.Equal(mode, request.Mode);
        Assert.Equal(scenarioId, request.ScenarioId);
    }

    [Fact]
    public void SendMessageRequest_ShouldInitialize_WithCorrectValues()
    {
        string content = "Hello, world!";

        SendMessageRequest request = new(content);

        Assert.Equal(content, request.Content);
    }

    [Fact]
    public void ConversationResponse_ShouldInitialize_WithCorrectValues()
    {
        string id = "conv-123";
        Language language = Language.French;
        ConversationMode mode = ConversationMode.Free;
        string? scenarioId = null;
        DateTime createdAt = DateTime.UtcNow;

        ConversationResponse response = new(id, language, mode, scenarioId, createdAt);

        Assert.Equal(id, response.Id);
        Assert.Equal(language, response.Language);
        Assert.Equal(mode, response.Mode);
        Assert.Equal(scenarioId, response.ScenarioId);
        Assert.Equal(createdAt, response.CreatedAt);
    }

    [Fact]
    public void MessageResponse_ShouldInitialize_WithCorrectValues()
    {
        string id = "msg-123";
        string content = "Bonjour!";
        MessageRole role = MessageRole.Assistant;
        List<CorrectionResponse> corrections = new();
        DateTime createdAt = DateTime.UtcNow;

        MessageResponse response = new(id, content, role, corrections, createdAt);

        Assert.Equal(id, response.Id);
        Assert.Equal(content, response.Content);
        Assert.Equal(role, response.Role);
        Assert.Equal(corrections, response.Corrections);
        Assert.Equal(createdAt, response.CreatedAt);
    }

    [Fact]
    public void CorrectionResponse_ShouldInitialize_WithCorrectValues()
    {
        string originalFragment = "I go yesterday";
        string suggestedFragment = "I went yesterday";
        string explanation = "Use past tense";
        ErrorType type = ErrorType.Grammar;

        CorrectionResponse response = new(originalFragment, suggestedFragment, explanation, type);

        Assert.Equal(originalFragment, response.OriginalFragment);
        Assert.Equal(suggestedFragment, response.SuggestedFragment);
        Assert.Equal(explanation, response.Explanation);
        Assert.Equal(type, response.Type);
    }

    [Fact]
    public void ScenarioResponse_ShouldInitialize_WithCorrectValues()
    {
        string id = "scenario-123";
        string title = "Hotel Check-in";
        string description = "Checking into a hotel room";

        ScenarioResponse response = new(id, title, description);

        Assert.Equal(id, response.Id);
        Assert.Equal(title, response.Title);
        Assert.Equal(description, response.Description);
    }
}
