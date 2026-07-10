using Idiomas.Core.Infrastructure.Service.Email;

namespace Idiomas.Tests.Core.Infrastructure.Service.Email;

public class EmailTemplateLoaderTest
{
    [Fact]
    public void Load_ReplacesPlaceholdersInTemplate()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "idiomas_templates_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        string templateContent = "<html><body>Hello {{UserName}}, click <a href=\"{{ResetLink}}\">here</a></body></html>";
        string templatePath = Path.Combine(tempDir, "TestEmail.html");
        File.WriteAllText(templatePath, templateContent);

        var loader = new EmailTemplateLoader(tempDir);

        var placeholders = new List<EmailTemplatePlaceholder>
        {
            new("UserName", "João"),
            new("ResetLink", "https://app.idiomas.com/reset?token=abc123")
        };

        string result = loader.Load("TestEmail.html", placeholders);

        Assert.Contains("Hello João", result);
        Assert.Contains("https://app.idiomas.com/reset?token=abc123", result);
        Assert.DoesNotContain("{{UserName}}", result);
        Assert.DoesNotContain("{{ResetLink}}", result);

        Directory.Delete(tempDir, true);
    }

    [Fact]
    public void Load_ThrowsFileNotFoundExceptionWhenTemplateDoesNotExist()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "idiomas_templates_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        var loader = new EmailTemplateLoader(tempDir);

        Assert.Throws<FileNotFoundException>(() => loader.Load("NonExistent.html", []));

        Directory.Delete(tempDir, true);
    }
}
