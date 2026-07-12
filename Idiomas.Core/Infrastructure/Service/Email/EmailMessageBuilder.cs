using Idiomas.Core.Interface.Service;

namespace Idiomas.Core.Infrastructure.Service.Email;

public class EmailMessageBuilder(EmailTemplateLoader templateLoader)
{
    private readonly EmailTemplateLoader _templateLoader = templateLoader;

    public virtual EmailMessage Build(string templateName, string subject, string recipient, params EmailTemplatePlaceholder[] placeholders)
    {
        string htmlBody = this._templateLoader.Load(templateName, placeholders);

        return new EmailMessage(recipient, subject, htmlBody);
    }
}
