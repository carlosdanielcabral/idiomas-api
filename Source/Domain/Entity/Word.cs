namespace IdiomasAPI.Source.Domain.Entity;

public class Word(int id, string word, string ipa, string userId, ICollection<Meaning> meanings)
{
    public int Id { get; private set; } = id;
    public string Name { get; private set; } = word;
    public string Ipa { get; private set; } = ipa;
    public string UserId { get; set; } = userId;
    public ICollection<Meaning> Meanings { get; private set; } = meanings;
}