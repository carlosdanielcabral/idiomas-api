namespace Idiomas.Core.Infrastructure.Service.Email;

public class EmailTemplateLoader(string templatesDirectory)
{
    private readonly string _templatesDirectory = templatesDirectory;

    public string Load(string templateName, IEnumerable<EmailTemplatePlaceholder> placeholders)
    {
        string filePath = Path.Combine(this._templatesDirectory, templateName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Email template '{templateName}' not found at '{filePath}'.", filePath);
        }

        string content = File.ReadAllText(filePath);

        foreach (EmailTemplatePlaceholder placeholder in placeholders)
        {
            content = content.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }

        return content;
    }
}
