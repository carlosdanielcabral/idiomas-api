using System.Net;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Idiomas.Tests.Core.Infrastructure.Service.Email;

public class SendGridClientServiceTest
{
    private readonly Mock<ISendGridClient> _sendGridClientMock = new();

    private IConfiguration BuildConfiguration(string senderAddress = "noreply@idiomas.app", string senderName = "Idiomas")
    {
        var configValues = new Dictionary<string, string?>
        {
            { "Email:SenderAddress", senderAddress },
            { "Email:SenderName", senderName }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    [Fact]
    public void Constructor_ThrowsWhenSenderAddressIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(senderAddress: null!);

        Assert.Throws<InvalidOperationException>(() => new SendGridClientService(this._sendGridClientMock.Object, config));
    }

    [Fact]
    public void Constructor_ThrowsWhenSenderNameIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(senderName: null!);

        Assert.Throws<InvalidOperationException>(() => new SendGridClientService(this._sendGridClientMock.Object, config));
    }

    [Fact]
    public async Task SendAsync_ThrowsApiExceptionWhenSendGridFails()
    {
        IConfiguration config = this.BuildConfiguration();

        Response failedResponse = new(HttpStatusCode.Unauthorized, new StringContent("error"), null);

        this._sendGridClientMock
            .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResponse);

        var service = new SendGridClientService(this._sendGridClientMock.Object, config);

        var message = new EmailMessage("user@example.com", "Subject", "<p>Body</p>");

        await Assert.ThrowsAsync<ApiException>(() => service.SendAsync(message));
    }

    [Fact]
    public async Task SendAsync_SendsEmailSuccessfully()
    {
        IConfiguration config = this.BuildConfiguration();

        Response successResponse = new(HttpStatusCode.OK, new StringContent(""), null);

        this._sendGridClientMock
            .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResponse)
            .Callback<SendGridMessage, CancellationToken>((msg, _) =>
            {
                Assert.Equal("user@example.com", msg.Personalizations[0].Tos[0].Email);
            });

        var service = new SendGridClientService(this._sendGridClientMock.Object, config);

        var message = new EmailMessage("user@example.com", "Subject", "<p>Body</p>");

        await service.SendAsync(message);

        this._sendGridClientMock.Verify(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
