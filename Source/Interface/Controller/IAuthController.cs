using IdiomasAPI.Source.Application.DTO.Auth;

namespace IdiomasAPI.Source.Interface.Controller;

public interface IAuthController
{
    public Task<IResult> MailPasswordLogin(MailPasswordLoginDTO dto);
}