using Idiomas.Core.Interface.Service;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Idiomas.Core.Infrastructure.Service.Email;

public class SmtpEmailService(ISmtpClient smtpClient, IConfiguration configuration) : IEmailService
{
    private readonly ISmtpClient _smtpClient = smtpClient;
    private readonly string _senderAddress = configuration["Email:SenderAddress"] ?? throw new InvalidOperationException("Email:SenderAddress is required");
    private readonly string _senderName = configuration["Email:SenderName"] ?? throw new InvalidOperationException("Email:SenderName is required");
    private readonly string _smtpHost = configuration["Smtp:Host"] ?? throw new InvalidOperationException("Smtp:Host is required");
    private readonly int _smtpPort = int.Parse(configuration["Smtp:Port"] ?? throw new InvalidOperationException("Smtp:Port is required"));

    public async Task SendAsync(EmailMessage message)
    {
        MimeMessage emailMessage = new();

        emailMessage.From.Add(new MailboxAddress(this._senderName, this._senderAddress));
        emailMessage.To.Add(MailboxAddress.Parse(message.To));
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new BodyBuilder { HtmlBody = message.HtmlBody }.ToMessageBody();

        await this._smtpClient.ConnectAsync(this._smtpHost, this._smtpPort, SecureSocketOptions.None);
        await this._smtpClient.SendAsync(emailMessage);
        await this._smtpClient.DisconnectAsync(true);
    }
}
