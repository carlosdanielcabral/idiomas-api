
using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Route;
using Idiomas.Core.Presentation.DTO.Auth;
using Idiomas.Core.Presentation.Http.Validator;
using Idiomas.Core.Presentation.Http.Validator.Auth;

namespace Idiomas.Core.Presentation.Http.Route;

public class AuthRoute(IAuthController controller) : IRoute
{
    private readonly IAuthController _controller = controller;

    public void Register(WebApplication app)
    {
        app.MapPost("/auth/login", this._controller.MailPasswordLogin)
            .Produces<LoginResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .WithValidation<MailPasswordLoginValidator, MailPasswordLoginDTO>();

        app.MapPost("/auth/google", this._controller.GoogleLogin)
            .Produces<LoginResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithValidation<GoogleLoginValidator, GoogleLoginDTO>();

        app.MapPost("/auth/forgot-password", this._controller.ForgotPassword)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .WithValidation<ForgotPasswordValidator, ForgotPasswordDTO>();

        app.MapPost("/auth/reset-password", this._controller.ResetPassword)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithValidation<ResetPasswordValidator, ResetPasswordDTO>();

        app.MapGet("/auth/verify-email", this._controller.VerifyEmail)
            .Produces(StatusCodes.Status302Found)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/auth/resend-verification", this._controller.ResendVerification)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .WithValidation<ResendVerificationValidator, ResendVerificationDTO>();

        app.MapGet("/auth/verify-email-change", this._controller.VerifyEmailChange)
            .Produces(StatusCodes.Status302Found)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
