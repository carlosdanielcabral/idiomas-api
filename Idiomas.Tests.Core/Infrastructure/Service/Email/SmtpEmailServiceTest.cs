using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Service;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using MimeKit;

namespace Idiomas.Tests.Core.Infrastructure.Service.Email;

public class SmtpEmailServiceTest
{
    private readonly Mock<ISmtpClient> _smtpClientMock = new();

    private IConfiguration BuildConfiguration(
        string senderAddress = "noreply@idiomas.app",
        string senderName = "Idiomas",
        string smtpHost = "localhost",
        string smtpPort = "1025")
    {
        var configValues = new Dictionary<string, string?>
        {
            { "Email:SenderAddress", senderAddress },
            { "Email:SenderName", senderName },
            { "Smtp:Host", smtpHost },
            { "Smtp:Port", smtpPort }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    [Fact]
    public void Constructor_ThrowsWhenSenderAddressIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(senderAddress: null!);

        Assert.Throws<InvalidOperationException>(() => new SmtpEmailService(this._smtpClientMock.Object, config));
    }

    [Fact]
    public void Constructor_ThrowsWhenSenderNameIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(senderName: null!);

        Assert.Throws<InvalidOperationException>(() => new SmtpEmailService(this._smtpClientMock.Object, config));
    }

    [Fact]
    public void Constructor_ThrowsWhenSmtpHostIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(smtpHost: null!);

        Assert.Throws<InvalidOperationException>(() => new SmtpEmailService(this._smtpClientMock.Object, config));
    }

    [Fact]
    public void Constructor_ThrowsWhenSmtpPortIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(smtpPort: null!);

        Assert.Throws<InvalidOperationException>(() => new SmtpEmailService(this._smtpClientMock.Object, config));
    }

    [Fact]
    public async Task SendAsync_SendsEmailSuccessfully()
    {
        IConfiguration config = this.BuildConfiguration();

        MimeMessage? sentMessage = null;

        this._smtpClientMock
            .Setup(client => client.ConnectAsync("localhost", 1025, SecureSocketOptions.None, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        this._smtpClientMock
            .Setup(client => client.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null))
            .Callback<MimeMessage, CancellationToken, ITransferProgress?>((msg, _, _) => sentMessage = msg)
            .ReturnsAsync(string.Empty);

        this._smtpClientMock
            .Setup(client => client.DisconnectAsync(true, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new SmtpEmailService(this._smtpClientMock.Object, config);

        var message = new EmailMessage("user@example.com", "Subject", "<p>Body</p>");

        await service.SendAsync(message);

        Assert.NotNull(sentMessage);
        Assert.Equal("user@example.com", sentMessage!.To.Mailboxes.First().Address);
        this._smtpClientMock.Verify(client => client.ConnectAsync("localhost", 1025, SecureSocketOptions.None, It.IsAny<CancellationToken>()), Times.Once);
        this._smtpClientMock.Verify(client => client.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
