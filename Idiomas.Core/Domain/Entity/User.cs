namespace Idiomas.Core.Domain.Entity;

public class User(string id, string name, string email, bool isEmailVerified)
{
    public string Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public bool IsEmailVerified { get; private set; } = isEmailVerified;

    public void MarkEmailAsVerified()
    {
        this.IsEmailVerified = true;
    }
}
