namespace Idiomas.Core.Application.DTO.User;

public record CreateUserDTO(string Name, string Email, string Password);

public record UpdateUserDTO(string Name, string Email, string Password);

public record UserDTO(string Id, string Name, string Email);
