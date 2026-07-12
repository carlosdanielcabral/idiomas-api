using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Conversation;

public sealed class ConversationNotFoundException() : ApiException(
    errorCode: "conversation:not-found",
    title: "Conversation not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested conversation was not found.")
{
}
