using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Infrastructure.Exceptions.Google;

public sealed class GoogleConfigurationMissingException() : ApiException(
    errorCode: "infrastructure:google-configuration-missing",
    title: "Google configuration missing",
    statusCode: HttpStatusCode.InternalServerError,
    detail: "Google OAuth configuration is missing or incomplete.")
{
}
