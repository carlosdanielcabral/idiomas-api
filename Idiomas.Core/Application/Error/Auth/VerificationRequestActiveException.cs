using System.Net;

namespace Idiomas.Core.Application.Error.Auth;

public sealed class VerificationRequestActiveException() : ApiException(
    errorCode: "auth:verification-request-active",
    title: "Verification request active",
    statusCode: HttpStatusCode.Conflict,
    detail: "An active verification request already exists. Check your email or wait for it to expire.")
{
}
