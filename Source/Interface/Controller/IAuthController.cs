using Idiomas.Source.Application.DTO.Auth;
using Idiomas.Source.Application.UseCase.AuthCase;

namespace Idiomas.Source.Interface.Controller;

public interface IAuthController
{
    public Task<IResult> MailPasswordLogin(MailPasswordLoginDTO dto, MailPasswordLogin useCase);
}