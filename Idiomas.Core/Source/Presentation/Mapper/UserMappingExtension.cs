using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Presentation.DTO.User;

namespace Idiomas.Source.Presentation.Mapper;

public static class UserMappingExtension
{
    public static UserResponseDTO ToResponseDTO(this User model)
    {
        return new UserResponseDTO() { Id = model.Id.ToString(), Name = model.Name, Email = model.Email };
    }
}