
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
            .Produces<MailPasswordLoginResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithValidation<MailPasswordLoginValidator, MailPasswordLoginDTO>();

        app.MapPost("/auth/forgot-password", this._controller.ForgotPassword)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .WithValidation<ForgotPasswordValidator, ForgotPasswordDTO>();

        app.MapPost("/auth/reset-password", this._controller.ResetPassword)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithValidation<ResetPasswordValidator, ResetPasswordDTO>();
    }
}