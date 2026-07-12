using Idiomas.Core.Infrastructure.Helper;

namespace Idiomas.Core.Domain.Entity;

public class User(string id, string name, string email, bool isEmailVerified)
{
    public string Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public bool IsEmailVerified { get; private set; } = isEmailVerified;

    public Guid IdAsGuid => Guid.Parse(this.Id);

    public static User Create(string name, string email, bool isEmailVerified)
    {
        return new User(UUIDGenerator.Generate(), name, email, isEmailVerified);
    }

    public void UpdateProfile(string name)
    {
        this.Name = name;
    }

    public void UpdateEmail(string newEmail)
    {
        this.Email = newEmail;
    }

    public bool IsEmailChanging(string newEmail)
    {
        return !string.Equals(newEmail, this.Email, StringComparison.OrdinalIgnoreCase);
    }

    public bool CanLogin()
    {
        return this.IsEmailVerified;
    }

    public void MarkEmailAsVerified()
    {
        this.IsEmailVerified = true;
    }
}
