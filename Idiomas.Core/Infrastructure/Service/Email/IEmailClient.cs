using SendGrid.Helpers.Mail;
using System.Net;

namespace Idiomas.Core.Infrastructure.Service.Email;

public interface IEmailClient
{
    Task<IEmailClientResponse> SendEmailAsync(SendGridMessage msg, CancellationToken cancellationToken = default);
}

public interface IEmailClientResponse
{
    bool IsSuccessStatusCode { get; }

    HttpStatusCode StatusCode { get; }

    HttpContent Body { get; }
}
