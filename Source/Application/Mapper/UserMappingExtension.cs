using IdiomasAPI.Source.Application.DTO.User;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Helper;

namespace IdiomasAPI.Source.Application.Mapper;

public static class UserMappingExtension
{
    public static User ToEntity(this CreateUserDTO dto)
    {
        return new User(UUIDGenerator.Generate(), dto.Name, dto.Email, dto.Password);
    }
}