using IdiomasAPI.Source.Application.DTO.User;

namespace IdiomasAPI.Source.Interface.Controller;

public interface IUserController
{
    public Task<IResult> SaveUser(CreateUserDTO dto);
}