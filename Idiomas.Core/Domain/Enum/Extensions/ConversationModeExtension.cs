namespace Idiomas.Core.Domain.Enum;

public static class ConversationModeExtension
{
    public static bool RequiresScenario(this ConversationMode mode)
    {
        return mode != ConversationMode.Free;
    }
}
