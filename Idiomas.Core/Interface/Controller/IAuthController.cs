using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Presentation.Http.Validator.Auth;

namespace Idiomas.Core.Interface.Controller;

public interface IAuthController
{
    public Task<IResult> MailPasswordLogin(HttpContext httpContext, MailPasswordLoginDTO dto, MailPasswordLoginValidator validator, MailPasswordLogin useCase);
}