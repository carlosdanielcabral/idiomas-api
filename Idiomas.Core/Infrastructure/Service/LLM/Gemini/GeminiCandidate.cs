using System.Text.Json.Serialization;

namespace Idiomas.Core.Infrastructure.Service.LLM.Gemini;

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; set; }
}
