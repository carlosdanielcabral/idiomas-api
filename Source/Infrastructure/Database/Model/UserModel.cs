namespace IdiomasAPI.Source.Infrastructure.Database.Model;

public class UserModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}