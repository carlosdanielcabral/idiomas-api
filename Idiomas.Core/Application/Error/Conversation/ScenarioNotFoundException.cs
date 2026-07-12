using System.Net;

namespace Idiomas.Core.Application.Error.Conversation;

public sealed class ScenarioNotFoundException() : ApiException(
    errorCode: "conversation:scenario-not-found",
    title: "Scenario not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested scenario was not found.")
{
}
