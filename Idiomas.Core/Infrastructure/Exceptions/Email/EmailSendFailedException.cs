using System.Net;
using Idiomas.Core.Exceptions;

namespace Idiomas.Core.Infrastructure.Exceptions.Email;

public sealed class EmailSendFailedException(string recipient) : ApiException(
    errorCode: "infrastructure:email-send-failed",
    title: "Email send failed",
    statusCode: HttpStatusCode.ServiceUnavailable,
    detail: $"Failed to send email to '{recipient}'.")
{
}
