using Idiomas.Core.Domain.Enum;

namespace Idiomas.Tests.Core.Domain.Enum;

public class EnumTest
{
    [Theory]
    [InlineData(Language.English, 0)]
    [InlineData(Language.Spanish, 1)]
    [InlineData(Language.French, 2)]
    [InlineData(Language.German, 3)]
    [InlineData(Language.Italian, 4)]
    [InlineData(Language.Portuguese, 5)]
    public void LanguageEnum_ShouldHaveExpectedValues(Language language, int expectedValue)
    {
        Assert.Equal(expectedValue, (int)language);
    }

    [Theory]
    [InlineData(ConversationMode.Free, 0)]
    [InlineData(ConversationMode.Guided, 1)]
    public void ConversationModeEnum_ShouldHaveExpectedValues(ConversationMode mode, int expectedValue)
    {
        Assert.Equal(expectedValue, (int)mode);
    }

    [Theory]
    [InlineData(MessageRole.User, 0)]
    [InlineData(MessageRole.Assistant, 1)]
    [InlineData(MessageRole.System, 2)]
    public void MessageRoleEnum_ShouldHaveExpectedValues(MessageRole role, int expectedValue)
    {
        Assert.Equal(expectedValue, (int)role);
    }

    [Theory]
    [InlineData(ErrorType.Grammar, 0)]
    [InlineData(ErrorType.Vocabulary, 1)]
    [InlineData(ErrorType.Pronunciation, 2)]
    [InlineData(ErrorType.Spelling, 3)]
    [InlineData(ErrorType.Syntax, 4)]
    public void ErrorTypeEnum_ShouldHaveExpectedValues(ErrorType type, int expectedValue)
    {
        Assert.Equal(expectedValue, (int)type);
    }
}
