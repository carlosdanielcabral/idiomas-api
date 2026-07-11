using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;

namespace Idiomas.Core.Interface.Controller;

public interface IAuthController
{
    public Task<IResult> MailPasswordLogin(HttpContext httpContext, MailPasswordLoginDTO dto, MailPasswordLogin useCase);

    public Task<IResult> ForgotPassword(ForgotPasswordDTO dto, ForgotPassword useCase);

    public Task<IResult> ResetPassword(ResetPasswordDTO dto, ResetPassword useCase);
}