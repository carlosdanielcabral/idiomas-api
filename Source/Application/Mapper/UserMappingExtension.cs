using Idiomas.Source.Application.DTO.User;
using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Infrastructure.Helper;

namespace Idiomas.Source.Application.Mapper;

public static class UserMappingExtension
{
    public static User ToEntity(this CreateUserDTO dto)
    {
        return new User(UUIDGenerator.Generate(), dto.Name, dto.Email, dto.Password);
    }
}