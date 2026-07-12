using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Conversation;

public sealed class ConversationEndedException() : ApiException(
    errorCode: "conversation:ended",
    title: "Conversation ended",
    statusCode: HttpStatusCode.Conflict,
    detail: "This conversation has already ended.")
{
}
