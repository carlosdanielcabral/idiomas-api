using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Conversation;

public sealed class MessageCreationFailedException() : ApiException(
    errorCode: "conversation:message-creation-failed",
    title: "Message creation failed",
    statusCode: HttpStatusCode.InternalServerError)
{
}
