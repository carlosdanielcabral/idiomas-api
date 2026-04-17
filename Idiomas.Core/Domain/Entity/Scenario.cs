using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Domain.Entity;

public class Scenario
{
    public string Id { get; private set; }
    public Language Language { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }

    public Scenario(string id, Language language, string title, string description)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        Id = id;
        Language = language;
        Title = title;
        Description = description;
        IsActive = true;
    }
}
