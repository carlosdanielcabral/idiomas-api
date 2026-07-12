using System.Net;

namespace Idiomas.Core.Application.Error.Conversation;

public sealed class ConversationNotFoundException() : ApiException(
    errorCode: "conversation:not-found",
    title: "Conversation not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested conversation was not found.")
{
}
