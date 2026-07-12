using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Conversation;

public sealed class ScenarioNotFoundException() : ApiException(
    errorCode: "conversation:scenario-not-found",
    title: "Scenario not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested scenario was not found.")
{
}
