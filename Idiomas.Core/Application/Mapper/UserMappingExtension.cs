using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Helper;

namespace Idiomas.Core.Application.Mapper;

public static class UserMappingExtension
{
    public static User ToEntity(this CreateUserDTO dto)
    {
        return new User(UUIDGenerator.Generate(), dto.Name, dto.Email, false);
    }

    public static User ToEntity(this UpdateUserDTO dto, string id)
    {
        return new User(id, dto.Name, dto.Email, false);
    }

    public static UserCredential ToCredentialEntity(this CreateUserDTO dto, string userId, string passwordHash)
    {
        return new UserCredential(
            UUIDGenerator.Generate(),
            userId,
            AuthProvider.Local,
            passwordHash,
            null
        );
    }
}
