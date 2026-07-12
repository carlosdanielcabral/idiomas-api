using System.Net;

namespace Idiomas.Core.Application.Error.Infrastructure;

public sealed class GoogleConfigurationMissingException() : ApiException(
    errorCode: "infrastructure:google-configuration-missing",
    title: "Google configuration missing",
    statusCode: HttpStatusCode.InternalServerError,
    detail: "Google OAuth configuration is missing or incomplete.")
{
}
