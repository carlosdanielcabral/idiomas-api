using System.Net;

namespace Idiomas.Core.Application.Error.Auth;

public sealed class AccountNotFoundException() : ApiException(
    errorCode: "auth:account-not-found",
    title: "Account not found",
    statusCode: HttpStatusCode.Unauthorized,
    detail: "No account was found for the provided credentials.")
{
}
