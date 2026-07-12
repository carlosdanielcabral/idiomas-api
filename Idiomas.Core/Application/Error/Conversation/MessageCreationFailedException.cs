using System.Net;

namespace Idiomas.Core.Application.Error.Conversation;

public sealed class MessageCreationFailedException() : ApiException(
    errorCode: "conversation:message-creation-failed",
    title: "Message creation failed",
    statusCode: HttpStatusCode.InternalServerError)
{
}
