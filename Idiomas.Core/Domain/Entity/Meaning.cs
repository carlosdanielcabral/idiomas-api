namespace Idiomas.Core.Domain.Entity;

public class Meaning(string id, string definition, string? example)
{
    public string Id { get; private set; } = id;
    public string Definition { get; private set; } = definition;
    public string? Example { get; private set; } = example;
}