using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Conversation;

public sealed class ConversationAccessDeniedException() : ApiException(
    errorCode: "conversation:access-denied",
    title: "Conversation access denied",
    statusCode: HttpStatusCode.Forbidden,
    detail: "You do not have permission to access this conversation.")
{
}
