using System.Text.Json.Serialization;

namespace Idiomas.Core.Infrastructure.Service.LLM.Gemini;

public class GeminiContentResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    [JsonPropertyName("corrections")]
    public List<GeminiCorrection>? Corrections { get; set; }
}
