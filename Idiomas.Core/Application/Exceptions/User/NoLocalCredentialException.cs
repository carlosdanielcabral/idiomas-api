using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.User;

public sealed class NoLocalCredentialException() : ApiException(
    errorCode: "user:no-local-credential",
    title: "No local credential",
    statusCode: HttpStatusCode.BadRequest,
    detail: "The user does not have a local credential and cannot update the password.")
{
}
