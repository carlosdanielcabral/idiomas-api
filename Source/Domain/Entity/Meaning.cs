namespace IdiomasAPI.Source.Domain.Entity;

public class Meaning(int id, string definition, string? example)
{
    public int Id { get; private set; } = id;
    public string Definition { get; private set; } = definition;
    public string? Example { get; private set; } = example;
}