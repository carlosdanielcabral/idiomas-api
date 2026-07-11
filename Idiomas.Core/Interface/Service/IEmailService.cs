namespace Idiomas.Core.Interface.Service;

public interface IEmailService
{
    public Task SendAsync(EmailMessage message);
}

public record EmailMessage(
    string To,
    string Subject,
    string HtmlBody
);
