using System.Net;

namespace Idiomas.Core.Application.Error.Auth;

public sealed class EmailNotVerifiedException() : ApiException(
    errorCode: "auth:email-not-verified",
    title: "Email not verified",
    statusCode: HttpStatusCode.Forbidden,
    detail: "The email has not been verified. Check your inbox to activate your account.")
{
}
