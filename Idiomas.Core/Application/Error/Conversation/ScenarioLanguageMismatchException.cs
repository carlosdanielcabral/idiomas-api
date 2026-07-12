using System.Net;

namespace Idiomas.Core.Application.Error.Conversation;

public sealed class ScenarioLanguageMismatchException() : ApiException(
    errorCode: "conversation:scenario-language-mismatch",
    title: "Scenario language mismatch",
    statusCode: HttpStatusCode.BadRequest,
    detail: "The scenario language does not match the conversation language.")
{
}
