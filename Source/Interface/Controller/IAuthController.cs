using IdiomasAPI.Source.Application.DTO.Auth;
using IdiomasAPI.Source.Application.UseCase.AuthCase;

namespace IdiomasAPI.Source.Interface.Controller;

public interface IAuthController
{
    public Task<IResult> MailPasswordLogin(MailPasswordLoginDTO dto, MailPasswordLogin useCase);
}