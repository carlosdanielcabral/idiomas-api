using System.Net;

namespace Idiomas.Core.Application.Error.Auth;

public sealed class TokenInvalidOrExpiredException() : ApiException(
    errorCode: "auth:token-invalid-or-expired",
    title: "Token invalid or expired",
    statusCode: HttpStatusCode.BadRequest,
    detail: "The token is invalid or has expired.")
{
}
