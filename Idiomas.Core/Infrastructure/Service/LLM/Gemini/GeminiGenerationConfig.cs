using System.Text.Json.Serialization;

namespace Idiomas.Core.Infrastructure.Service.LLM.Gemini;

public class GeminiGenerationConfig
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("responseMimeType")]
    public string ResponseMimeType { get; set; } = "application/json";
}
