using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;

namespace Idiomas.Api.Interface.Controller;

public interface IAuthController
{
    public Task<IResult> MailPasswordLogin(MailPasswordLoginDTO dto, MailPasswordLogin useCase);
}