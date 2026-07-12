using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Mapper;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;

namespace Idiomas.Tests.Core.Application.Mapper;

public class UserMappingExtensionTest
{
    [Fact]
    public void ToEntity_FromCreateUserDTO_MapsCorrectly()
    {
        CreateUserDTO dto = new("Test User", "test@example.com", "password123");

        User entity = dto.ToEntity();

        Assert.Equal("Test User", entity.Name);
        Assert.Equal("test@example.com", entity.Email);
        Assert.NotEmpty(entity.Id);
    }

    [Fact]
    public void ToEntity_FromUpdateUserDTO_MapsCorrectly()
    {
        string userId = Guid.NewGuid().ToString();
        UpdateUserDTO dto = new("Updated Name", "updated@example.com", null);

        User entity = dto.ToEntity(userId);

        Assert.Equal(userId, entity.Id);
        Assert.Equal("Updated Name", entity.Name);
        Assert.Equal("updated@example.com", entity.Email);
    }

    [Fact]
    public void Create_WithLocalProvider_CreatesCorrectly()
    {
        string userId = Guid.NewGuid().ToString();

        UserCredential credential = UserCredential.Create(userId, AuthProvider.Local, "hashed-password");

        Assert.Equal(userId, credential.UserId);
        Assert.Equal(AuthProvider.Local, credential.Provider);
        Assert.Equal("hashed-password", credential.PasswordHash);
        Assert.Null(credential.ExternalSubject);
    }
}
