using SendGrid;
using SendGrid.Helpers.Mail;

namespace Idiomas.Core.Infrastructure.Service.Email;

public class SendGridClientAdapter(SendGridClient sendGridClient) : IEmailClient
{
    private readonly SendGridClient _sendGridClient = sendGridClient;

    public async Task<IEmailClientResponse> SendEmailAsync(SendGridMessage msg, CancellationToken cancellationToken = default)
    {
        Response response = await this._sendGridClient.SendEmailAsync(msg, cancellationToken);

        return new SendGridClientResponseAdapter(response);
    }
}

public class SendGridClientResponseAdapter(Response response) : IEmailClientResponse
{
    private readonly Response _response = response;

    public bool IsSuccessStatusCode => this._response.IsSuccessStatusCode;

    public System.Net.HttpStatusCode StatusCode => this._response.StatusCode;

    public HttpContent Body => this._response.Body;
}
