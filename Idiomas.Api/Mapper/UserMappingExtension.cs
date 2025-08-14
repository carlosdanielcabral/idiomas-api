using Idiomas.Core.Domain.Entity;
using Idiomas.Api.DTO.User;

namespace Idiomas.Api.Mapper;

public static class UserMappingExtension
{
    public static UserResponseDTO ToResponseDTO(this User model)
    {
        return new UserResponseDTO() { Id = model.Id.ToString(), Name = model.Name, Email = model.Email };
    }
}