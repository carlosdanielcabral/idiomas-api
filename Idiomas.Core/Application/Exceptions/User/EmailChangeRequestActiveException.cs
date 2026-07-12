using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.User;

public sealed class EmailChangeRequestActiveException() : ApiException(
    errorCode: "user:email-change-request-active",
    title: "Email change request active",
    statusCode: HttpStatusCode.Conflict,
    detail: "An active email change request already exists. Check your email or wait for it to expire.")
{
}
