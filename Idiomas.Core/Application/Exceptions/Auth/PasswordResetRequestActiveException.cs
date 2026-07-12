using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Auth;

public sealed class PasswordResetRequestActiveException() : ApiException(
    errorCode: "auth:password-reset-request-active",
    title: "Password reset request active",
    statusCode: HttpStatusCode.Conflict,
    detail: "An active password reset request already exists. Check your email or wait for it to expire.")
{
}
