using System.Net;

namespace Idiomas.Core.Application.Error.Auth;

public sealed class GoogleEmailNotVerifiedException() : ApiException(
    errorCode: "auth:google-email-not-verified",
    title: "Google email not verified",
    statusCode: HttpStatusCode.Unauthorized,
    detail: "The email has not been verified by Google.")
{
}
