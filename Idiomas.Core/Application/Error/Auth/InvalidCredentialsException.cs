using System.Net;

namespace Idiomas.Core.Application.Error.Auth;

public sealed class InvalidCredentialsException() : ApiException(
    errorCode: "auth:invalid-credentials",
    title: "Invalid credentials",
    statusCode: HttpStatusCode.BadRequest,
    detail: "The email or password is invalid.")
{
}
