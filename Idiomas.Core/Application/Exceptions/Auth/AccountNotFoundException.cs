using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Application.Exceptions.Auth;

public sealed class AccountNotFoundException() : ApiException(
    errorCode: "auth:account-not-found",
    title: "Account not found",
    statusCode: HttpStatusCode.Unauthorized,
    detail: "No account was found for the provided credentials.")
{
}
