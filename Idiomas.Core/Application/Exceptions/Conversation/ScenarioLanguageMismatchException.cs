using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Conversation;

public sealed class ScenarioLanguageMismatchException() : ApiException(
    errorCode: "conversation:scenario-language-mismatch",
    title: "Scenario language mismatch",
    statusCode: HttpStatusCode.BadRequest,
    detail: "The scenario language does not match the conversation language.")
{
}
