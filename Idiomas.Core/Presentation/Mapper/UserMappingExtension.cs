using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Presentation.DTO.User;

namespace Idiomas.Core.Presentation.Mapper;

public static class UserMappingExtension
{
    public static UserResponseDTO ToResponseDTO(this User model)
    {
        return new UserResponseDTO() { Id = model.Id.ToString(), Name = model.Name, Email = model.Email };
    }
}