using Idiomas.Core.Application.Error;
using Idiomas.Core.Infrastructure.Service.Email;
using Idiomas.Core.Interface.Service;
using Microsoft.Extensions.Configuration;
using Moq;
using SendGrid.Helpers.Mail;

namespace Idiomas.Tests.Core.Infrastructure.Service.Email;

public class SendGridEmailServiceTest
{
    private readonly Mock<IEmailClient> _sendGridClientMock = new();

    private IConfiguration BuildConfiguration(string apiKey = "SG.test", string senderAddress = "noreply@idiomas.app", string senderName = "Idiomas")
    {
        var configValues = new Dictionary<string, string?>
        {
            { "SendGrid:ApiKey", apiKey },
            { "Email:SenderAddress", senderAddress },
            { "Email:SenderName", senderName }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    [Fact]
    public void Constructor_ThrowsWhenApiKeyIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(apiKey: null!);

        Assert.Throws<InvalidOperationException>(() => new SendGridEmailService(this._sendGridClientMock.Object, config));
    }

    [Fact]
    public void Constructor_ThrowsWhenSenderAddressIsMissing()
    {
        IConfiguration config = this.BuildConfiguration(senderAddress: null!);

        Assert.Throws<InvalidOperationException>(() => new SendGridEmailService(this._sendGridClientMock.Object, config));
    }

    [Fact]
    public async Task SendAsync_ThrowsApiExceptionWhenSendGridFails()
    {
        IConfiguration config = this.BuildConfiguration();

        var failedResponse = new Mock<IEmailClientResponse>();
        failedResponse.SetupGet(r => r.IsSuccessStatusCode).Returns(false);
        failedResponse.SetupGet(r => r.StatusCode).Returns(System.Net.HttpStatusCode.Unauthorized);
        failedResponse.SetupGet(r => r.Body).Returns(new StringContent("error"));

        this._sendGridClientMock
            .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResponse.Object);

        var service = new SendGridEmailService(this._sendGridClientMock.Object, config);

        var message = new EmailMessage("user@example.com", "Subject", "<p>Body</p>");

        await Assert.ThrowsAsync<ApiException>(() => service.SendAsync(message));
    }

    [Fact]
    public async Task SendAsync_SendsEmailSuccessfully()
    {
        IConfiguration config = this.BuildConfiguration();

        var successResponse = new Mock<IEmailClientResponse>();
        successResponse.SetupGet(r => r.IsSuccessStatusCode).Returns(true);
        successResponse.SetupGet(r => r.StatusCode).Returns(System.Net.HttpStatusCode.OK);
        successResponse.SetupGet(r => r.Body).Returns(new StringContent(""));

        this._sendGridClientMock
            .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResponse.Object)
            .Callback<SendGridMessage, CancellationToken>((msg, _) =>
            {
                Assert.Equal("user@example.com", msg.Personalizations[0].Tos[0].Email);
            });

        var service = new SendGridEmailService(this._sendGridClientMock.Object, config);

        var message = new EmailMessage("user@example.com", "Subject", "<p>Body</p>");

        await service.SendAsync(message);

        this._sendGridClientMock.Verify(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
