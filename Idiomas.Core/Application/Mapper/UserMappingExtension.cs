using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Application.Mapper;

public static class UserMappingExtension
{
    public static User ToEntity(this CreateUserDTO dto)
    {
        return User.Create(dto.Name, dto.Email, false);
    }

    public static User ToEntity(this UpdateUserDTO dto, string id)
    {
        return new User(id, dto.Name, dto.Email, false);
    }
}
