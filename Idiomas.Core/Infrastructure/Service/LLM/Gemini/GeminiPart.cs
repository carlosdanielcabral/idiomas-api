using System.Text.Json.Serialization;

namespace Idiomas.Core.Infrastructure.Service.LLM.Gemini;

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
