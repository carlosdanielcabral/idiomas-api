using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Auth;

public sealed class TokenInvalidOrExpiredException() : ApiException(
    errorCode: "auth:token-invalid-or-expired",
    title: "Token invalid or expired",
    statusCode: HttpStatusCode.BadRequest,
    detail: "The token is invalid or has expired.")
{
}
