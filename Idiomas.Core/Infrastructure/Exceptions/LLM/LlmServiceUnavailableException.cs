using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Infrastructure.Exceptions.LLM;

public sealed class LlmServiceUnavailableException() : ApiException(
    errorCode: "infrastructure:llm-service-unavailable",
    title: "AI service unavailable",
    statusCode: HttpStatusCode.ServiceUnavailable,
    detail: "The AI service is temporarily unavailable. Please try again later.")
{
}
