using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Idiomas.Core.Application.DTO.Conversation;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.LLM;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;

namespace Idiomas.Core.Infrastructure.Service.LLM;

public class GeminiConversationLLMService : IConversationLLMService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly IConfiguration _configuration;

    public GeminiConversationLLMService(HttpClient httpClient, IConfiguration configuration)
    {
        this._httpClient = httpClient;
        this._configuration = configuration;
        this._apiKey = configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey is required");
        this._model = configuration["Gemini:Model"] ?? "gemini-2.0-flash";
    }

    public async Task<ConversationLLMResponse> SendMessageAsync(
        Conversation conversation,
        string userMessage,
        string? scenarioDescription)
    {
        GeminiRequest request = this.BuildRequest(conversation, userMessage, scenarioDescription);

        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{this._model}:generateContent?key={this._apiKey}";

        const int maxRetries = 3;
        const int baseDelayMs = 1000;
        
        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                HttpResponseMessage response = await this._httpClient.PostAsJsonAsync(
                    url,
                    request,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );
        
                if (response.IsSuccessStatusCode)
                {
                    GeminiResponse? geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                    string content = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "{}";

                    return this.ParseResponse(content);
                }

                // Handle retryable status codes
                if (this.IsRetryableStatusCode(response.StatusCode) && attempt < maxRetries)
                {
                    int delayMs = this.CalculateExponentialBackoffWithJitter(baseDelayMs, attempt);
                    await Task.Delay(delayMs);

                    continue;
                }

                // For non-retryable status codes or max retries reached, throw proper ApiException
                string errorContent = await response.Content.ReadAsStringAsync();

                throw new ApiException(
                    $"Failed to process message with AI service. Status: {response.StatusCode}. Please try again later.",
                    HttpStatusCode.ServiceUnavailable
                );
            }
            catch (HttpRequestException ex) when (attempt < maxRetries)
            {
                // Handle network errors with retry
                int delayMs = this.CalculateExponentialBackoffWithJitter(baseDelayMs, attempt);
                await Task.Delay(delayMs);

                continue;
            }
        }

        // This should not be reached, but handle the case where all retries fail
        throw new ApiException(
            "AI service is temporarily unavailable. Please try again later.",
            HttpStatusCode.ServiceUnavailable
        );
    }

    private bool IsRetryableStatusCode(HttpStatusCode statusCode)
    {
        HttpStatusCode[] retryableStatusCodes = 
        {
            HttpStatusCode.TooManyRequests, // 429
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout // 504
        };

        return retryableStatusCodes.Contains(statusCode);
    }

    private int CalculateExponentialBackoffWithJitter(int baseDelayMs, int attempt)
    {
        Random random = new();
        double exponentialDelay = baseDelayMs * Math.Pow(2, attempt);
        double jitter = random.NextDouble() * 0.1 * exponentialDelay; // 10% jitter
        return (int)(exponentialDelay + jitter);
    }

    private GeminiRequest BuildRequest(
        Conversation conversation,
        string userMessage,
        string? scenarioDescription)
    {
        List<GeminiContent> contents = new();

        string systemInstruction = LLMPrompts.SystemPrompt;

        if (!string.IsNullOrEmpty(scenarioDescription))
        {
            systemInstruction += $"\n\nCurrent scenario: {scenarioDescription}. Stay faithful to this context during the conversation.";
        }

        systemInstruction += $"\n\nThe user is practicing {conversation.Language}. Respond exclusively in this language.";

        string contextLimitString = this._configuration["Conversation:ContextLimit"];
int contextLimit = string.IsNullOrEmpty(contextLimitString) ? 10 : int.Parse(contextLimitString);

        foreach (Message message in conversation.Messages.OrderBy(m => m.CreatedAt).TakeLast(contextLimit))
        {
            string role = message.Role switch
            {
                MessageRole.User => "user",
                MessageRole.Assistant => "model",
                _ => "user"
            };

            contents.Add(new GeminiContent
            {
                Role = role,
                Parts = new List<GeminiPart> { new GeminiPart { Text = message.Content } }
            });
        }

        contents.Add(new GeminiContent
        {
            Role = "user",
            Parts = new List<GeminiPart> { new GeminiPart { Text = userMessage } }
        });

        return new GeminiRequest
        {
            SystemInstruction = new GeminiContent
            {
                Parts = new List<GeminiPart> { new GeminiPart { Text = systemInstruction } }
            },
            Contents = contents,
            GenerationConfig = new GeminiGenerationConfig
            {
                Temperature = 0.7,
                ResponseMimeType = "application/json"
            }
        };
    }

    private ConversationLLMResponse ParseResponse(string content)
    {
        try
        {
            GeminiContentResponse? parsed = JsonSerializer.Deserialize<GeminiContentResponse>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            List<CorrectionResponse> corrections = parsed?.Corrections?.Select(c => new CorrectionResponse(
                c.OriginalFragment,
                c.SuggestedFragment,
                c.Explanation,
                Enum.Parse<ErrorType>(c.Type)
            )).ToList() ?? new List<CorrectionResponse>();

            return new ConversationLLMResponse(
                parsed?.Response ?? "I'm sorry, I couldn't process that.",
                corrections);
        }
        catch (JsonException)
        {
            return new ConversationLLMResponse(
                "I'm sorry, I couldn't process that response.",
                new List<CorrectionResponse>());
        }
    }
}

public class GeminiRequest
{
    [JsonPropertyName("system_instruction")]
    public GeminiContent? SystemInstruction { get; set; }

    [JsonPropertyName("contents")]
    public List<GeminiContent> Contents { get; set; } = new();

    [JsonPropertyName("generationConfig")]
    public GeminiGenerationConfig GenerationConfig { get; set; } = new();
}

public class GeminiContent
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; set; } = new();
}

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class GeminiGenerationConfig
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("responseMimeType")]
    public string ResponseMimeType { get; set; } = "application/json";
}

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }
}

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; set; }
}

public class GeminiContentResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    [JsonPropertyName("corrections")]
    public List<GeminiCorrection>? Corrections { get; set; }
}

public class GeminiCorrection
{
    public string OriginalFragment { get; set; } = string.Empty;

    public string SuggestedFragment { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}
