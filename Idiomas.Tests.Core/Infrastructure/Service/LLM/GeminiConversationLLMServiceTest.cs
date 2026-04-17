using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Service.LLM;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace Idiomas.Tests.Core.Infrastructure.Service.LLM;

public class GeminiConversationLLMServiceTest
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly GeminiConversationLLMService _service;

    public GeminiConversationLLMServiceTest()
    {
        this._httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        this._httpClient = new HttpClient(this._httpMessageHandlerMock.Object);
        this._configurationMock = new Mock<IConfiguration>();

        this._configurationMock
            .Setup(config => config["Gemini:ApiKey"])
            .Returns("test-api-key");

        this._configurationMock
            .Setup(config => config["Gemini:Model"])
            .Returns("gemini-2.0-flash");

        this._service = new GeminiConversationLLMService(this._httpClient, this._configurationMock.Object);
    }

    [Fact]
    public async Task SendMessageAsync_With429Response_ShouldRetryAndSucceed()
    {
        // Arrange
        Conversation conversation = new("conv-123", "user-123", Language.English, ConversationMode.Free);
        string userMessage = "Hello";
        string? scenarioDescription = null;

        // Setup: First 2 calls return 429, third call succeeds
        var geminiResponse = new
        {
            candidates = new[]
            {
                new
                {
                    content = new
                    {
                        parts = new[]
                        {
                            new { text = "{\"response\":\"Hello! How can I help you?\"}" }
                        }
                    }
                }
            }
        };

        var callCount = 0;
        this._httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount <= 2)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
                    return response;
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = JsonContent.Create(geminiResponse);
                    return response;
                }
            });

        // Act
        var result = await this._service.SendMessageAsync(conversation, userMessage, scenarioDescription);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Hello! How can I help you?", result.Content);
        Assert.Equal(3, callCount); // Should have made 3 attempts
    }

    [Fact]
    public async Task SendMessageAsync_WithMaxRetriesExceeded_ShouldThrowException()
    {
        // Arrange
        Conversation conversation = new("conv-123", "user-123", Language.English, ConversationMode.Free);
        string userMessage = "Hello";
        string? scenarioDescription = null;

        // Setup: All calls return 429
        this._httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.TooManyRequests));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => this._service.SendMessageAsync(conversation, userMessage, scenarioDescription));

        Assert.Contains("Failed to get response from Gemini API after maximum retries", exception.Message);
    }

    [Fact]
    public async Task SendMessageAsync_WithNonRetryableError_ShouldThrowImmediately()
    {
        // Arrange
        Conversation conversation = new("conv-123", "user-123", Language.English, ConversationMode.Free);
        string userMessage = "Hello";
        string? scenarioDescription = null;

        // Setup: Returns 401 (non-retryable)
        this._httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => this._service.SendMessageAsync(conversation, userMessage, scenarioDescription));
    }
}
