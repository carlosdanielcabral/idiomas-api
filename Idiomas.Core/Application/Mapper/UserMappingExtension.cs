using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Helper;

namespace Idiomas.Core.Application.Mapper;

public static class UserMappingExtension
{
    public static User ToEntity(this CreateUserDTO dto)
    {
        return new User(UUIDGenerator.Generate(), dto.Name, dto.Email, dto.Password);
    }
}