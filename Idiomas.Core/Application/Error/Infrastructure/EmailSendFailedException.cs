using System.Net;

namespace Idiomas.Core.Application.Error.Infrastructure;

public sealed class EmailSendFailedException(string recipient) : ApiException(
    errorCode: "infrastructure:email-send-failed",
    title: "Email send failed",
    statusCode: HttpStatusCode.ServiceUnavailable,
    detail: $"Failed to send email to '{recipient}'.")
{
}
