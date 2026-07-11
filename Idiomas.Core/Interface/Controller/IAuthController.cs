using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;

namespace Idiomas.Core.Interface.Controller;

public interface IAuthController
{
    public Task<IResult> MailPasswordLogin(HttpContext httpContext, MailPasswordLoginDTO dto, MailPasswordLogin useCase);

    public Task<IResult> GoogleLogin(HttpContext httpContext, GoogleLoginDTO dto, GoogleLogin useCase);

    public Task<IResult> ForgotPassword(ForgotPasswordDTO dto, ForgotPassword useCase);

    public Task<IResult> ResetPassword(ResetPasswordDTO dto, ResetPassword useCase);

    public Task<IResult> VerifyEmail(string token, VerifyEmail useCase);

    public Task<IResult> ResendVerification(ResendVerificationDTO dto, ResendVerification useCase);

    public Task<IResult> VerifyEmailChange(string token, VerifyEmailChange useCase);
}
