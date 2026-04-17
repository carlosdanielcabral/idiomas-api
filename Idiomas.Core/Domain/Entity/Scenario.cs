using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Domain.Entity;

public class Scenario(string id, Language language, string title, string description)
{
    public string Id { get; private set; } = id;
    public Language Language { get; private set; } = language;
    public string Title { get; private set; } = title;
    public string Description { get; private set; } = description;
    public bool IsActive { get; private set; } = true;
}
