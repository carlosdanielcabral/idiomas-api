using System.Net;

namespace Idiomas.Core.Application.Error.User;

public sealed class NoLocalCredentialException() : ApiException(
    errorCode: "user:no-local-credential",
    title: "No local credential",
    statusCode: HttpStatusCode.BadRequest,
    detail: "The user does not have a local credential and cannot update the password.")
{
}
