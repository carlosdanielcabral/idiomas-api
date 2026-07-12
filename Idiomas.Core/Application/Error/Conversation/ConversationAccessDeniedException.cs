using System.Net;

namespace Idiomas.Core.Application.Error.Conversation;

public sealed class ConversationAccessDeniedException() : ApiException(
    errorCode: "conversation:access-denied",
    title: "Conversation access denied",
    statusCode: HttpStatusCode.Forbidden,
    detail: "You do not have permission to access this conversation.")
{
}
