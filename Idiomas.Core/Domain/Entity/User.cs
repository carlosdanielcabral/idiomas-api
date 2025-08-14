namespace Idiomas.Source.Domain.Entity;

public class User(string id, string name, string email, string password)
{
    public string Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public string Password { get; set; } = password;
}