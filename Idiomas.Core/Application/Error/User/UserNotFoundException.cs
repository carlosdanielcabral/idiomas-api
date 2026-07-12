using System.Net;

namespace Idiomas.Core.Application.Error.User;

public sealed class UserNotFoundException() : ApiException(
    errorCode: "user:not-found",
    title: "User not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested user was not found.")
{
}
