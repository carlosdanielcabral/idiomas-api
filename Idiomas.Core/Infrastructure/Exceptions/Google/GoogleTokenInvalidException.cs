using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Infrastructure.Exceptions.Google;

public sealed class GoogleTokenInvalidException() : ApiException(
    errorCode: "infrastructure:google-token-invalid",
    title: "Google token invalid",
    statusCode: HttpStatusCode.Unauthorized,
    detail: "The provided Google token is invalid or has expired.")
{
}
