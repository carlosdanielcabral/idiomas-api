namespace Idiomas.Core.Infrastructure.Service.LLM.Gemini;

public class GeminiCorrection
{
    public string OriginalFragment { get; set; } = string.Empty;

    public string SuggestedFragment { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}
