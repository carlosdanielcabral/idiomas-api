using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.User;

public sealed class UserNotFoundException() : ApiException(
    errorCode: "user:not-found",
    title: "User not found",
    statusCode: HttpStatusCode.NotFound,
    detail: "The requested user was not found.")
{
}
