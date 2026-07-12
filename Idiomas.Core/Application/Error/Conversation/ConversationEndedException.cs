using System.Net;

namespace Idiomas.Core.Application.Error.Conversation;

public sealed class ConversationEndedException() : ApiException(
    errorCode: "conversation:ended",
    title: "Conversation ended",
    statusCode: HttpStatusCode.Conflict,
    detail: "This conversation has already ended.")
{
}
