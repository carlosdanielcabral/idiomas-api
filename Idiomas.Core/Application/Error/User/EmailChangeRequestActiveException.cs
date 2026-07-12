using System.Net;

namespace Idiomas.Core.Application.Error.User;

public sealed class EmailChangeRequestActiveException() : ApiException(
    errorCode: "user:email-change-request-active",
    title: "Email change request active",
    statusCode: HttpStatusCode.Conflict,
    detail: "An active email change request already exists. Check your email or wait for it to expire.")
{
}
