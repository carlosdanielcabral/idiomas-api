using SendGrid.Helpers.Mail;
using System.Net;

namespace Idiomas.Core.Infrastructure.Service.Email;

public interface ISendGridClient
{
    Task<ISendGridClientResponse> SendEmailAsync(SendGridMessage msg, CancellationToken cancellationToken = default);
}

public interface ISendGridClientResponse
{
    bool IsSuccessStatusCode { get; }

    HttpStatusCode StatusCode { get; }

    HttpContent Body { get; }
}
