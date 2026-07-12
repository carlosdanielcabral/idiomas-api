using System.Net;

namespace Idiomas.Core.Application.Error.User;

public sealed class EmailAlreadyInUseException() : ApiException(
    errorCode: "user:email-already-in-use",
    title: "Email already in use",
    statusCode: HttpStatusCode.Conflict,
    detail: "The email address is already associated with another account.")
{
}
