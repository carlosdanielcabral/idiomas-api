using System.Net;
using Idiomas.Core.Application.DTO.User;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.UserCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Moq;

namespace Idiomas.Tests.Core.Application.UseCase.UserCase;

public class CreateUserTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IHash> _hashMock;
    private readonly CreateUser _sut;

    public CreateUserTest()
    {
        this._userRepositoryMock = new Mock<IUserRepository>();
        this._hashMock = new Mock<IHash>();
        this._sut = new CreateUser(_userRepositoryMock.Object, _hashMock.Object);
    }

    [Fact]
    public async Task Execute_ShouldCreateUser_WhenEmailIsUnique()
    {
        var createUserDTO = new CreateUserDTO("Test User", "test@example.com", "password123");

        this._userRepositoryMock
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync((User) null!);

        this._hashMock
            .Setup(hash => hash.Hash(It.IsAny<string>()))
            .Returns("hashed_password");

        this._userRepositoryMock
            .Setup(repository => repository.Insert(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        var result = await _sut.Execute(createUserDTO);

        Assert.NotNull(result);
        Assert.Equal(createUserDTO.Name, result.Name);
        Assert.Equal(createUserDTO.Email, result.Email);
        Assert.Equal("hashed_password", result.Password);

        this._userRepositoryMock.Verify(repository => repository.Insert(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowApiException_WhenEmailAlreadyExists()
    {
        CreateUserDTO createUserDTO = new("Test User", "test@example.com", "password123");
        User existingUser = new("1", "Existing User", "test@example.com", "hashed_password");

        this._userRepositoryMock
            .Setup(rrepository => rrepository.GetByEmail(createUserDTO.Email))
            .ReturnsAsync(existingUser);

        var exception = await Assert.ThrowsAsync<ApiException>(() => _sut.Execute(createUserDTO));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Equal("E-mail já cadastrado", exception.Message);

        this._userRepositoryMock.Verify(repository => repository.Insert(It.IsAny<User>()), Times.Never);
    }
}