using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Auth;

public sealed class InvalidCredentialsException() : ApiException(
    errorCode: "auth:invalid-credentials",
    title: "Invalid credentials",
    statusCode: HttpStatusCode.BadRequest,
    detail: "The email or password is invalid.")
{
}
