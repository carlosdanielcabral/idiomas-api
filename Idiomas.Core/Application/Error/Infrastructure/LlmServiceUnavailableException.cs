using System.Net;

namespace Idiomas.Core.Application.Error.Infrastructure;

public sealed class LlmServiceUnavailableException() : ApiException(
    errorCode: "infrastructure:llm-service-unavailable",
    title: "AI service unavailable",
    statusCode: HttpStatusCode.ServiceUnavailable,
    detail: "The AI service is temporarily unavailable. Please try again later.")
{
}
