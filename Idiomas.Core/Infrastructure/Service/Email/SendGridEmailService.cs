using System.Net;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Idiomas.Core.Infrastructure.Service.Email;

public class SendGridEmailService(ISendGridClient sendGridClient, IConfiguration configuration) : IEmailService
{
    private readonly ISendGridClient _sendGridClient = sendGridClient;
    private readonly string _apiKey = configuration["SendGrid:ApiKey"] ?? throw new InvalidOperationException("SendGrid:ApiKey is required");
    private readonly string _senderAddress = configuration["Email:SenderAddress"] ?? throw new InvalidOperationException("Email:SenderAddress is required");
    private readonly string _senderName = configuration["Email:SenderName"] ?? throw new InvalidOperationException("Email:SenderName is required");

    public async Task SendAsync(EmailMessage message)
    {
        SendGridMessage emailMessage = MailHelper.CreateSingleEmail(
            new EmailAddress(this._senderAddress, this._senderName),
            new EmailAddress(message.To),
            message.Subject,
            plainTextContent: "",
            htmlContent: message.HtmlBody
        );

        ISendGridClientResponse response = await this._sendGridClient.SendEmailAsync(emailMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException($"Failed to send email to '{message.To}'. Status: {response.StatusCode}", HttpStatusCode.ServiceUnavailable);
        }
    }
}
