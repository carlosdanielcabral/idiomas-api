using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.User;

public sealed class EmailAlreadyInUseException() : ApiException(
    errorCode: "user:email-already-in-use",
    title: "Email already in use",
    statusCode: HttpStatusCode.Conflict,
    detail: "The email address is already associated with another account.")
{
}
