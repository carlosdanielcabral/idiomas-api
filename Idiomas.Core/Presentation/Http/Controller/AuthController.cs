using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Service;
using Idiomas.Core.Presentation.DTO.Auth;
using Idiomas.Core.Presentation.Mapper;
using Microsoft.Extensions.Configuration;

namespace Idiomas.Core.Presentation.Http.Controller;

public class AuthController(IToken tokenGenerator, IConfiguration configuration) : IAuthController
{
    private readonly IToken _tokenGenerator = tokenGenerator;
    private readonly string _frontendUrl = configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl is not configured");

    public async Task<IResult> MailPasswordLogin(HttpContext httpContext, MailPasswordLoginDTO dto, MailPasswordLogin useCase)
    {
        User user = await useCase.Execute(dto);

        LoginResponseDTO response = new()
        {
            User = user.ToResponseDTO(),
            Token = this._tokenGenerator.Generate(user)
        };

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        };

        httpContext.Response.Cookies.Append("Authorization", response.Token, cookieOptions);

        return TypedResults.Ok(response);
    }

    public async Task<IResult> GoogleLogin(HttpContext httpContext, GoogleLoginDTO dto, GoogleLogin useCase)
    {
        User user = await useCase.Execute(dto);

        LoginResponseDTO response = new()
        {
            User = user.ToResponseDTO(),
            Token = this._tokenGenerator.Generate(user)
        };

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        };

        httpContext.Response.Cookies.Append("Authorization", response.Token, cookieOptions);

        return TypedResults.Ok(response);
    }

    public async Task<IResult> ForgotPassword(ForgotPasswordDTO dto, ForgotPassword useCase)
    {
        await useCase.Execute(dto);

        return TypedResults.Ok();
    }

    public async Task<IResult> ResetPassword(ResetPasswordDTO dto, ResetPassword useCase)
    {
        await useCase.Execute(dto);

        return TypedResults.Ok();
    }

    public async Task<IResult> VerifyEmail(string token, VerifyEmail useCase)
    {
        try
        {
            await useCase.Execute(token);

            return Results.Redirect($"{this._frontendUrl}/email-verified?status=success");
        }
        catch (ApiException)
        {
            return Results.Redirect($"{this._frontendUrl}/email-verified?status=invalid");
        }
    }

    public async Task<IResult> ResendVerification(ResendVerificationDTO dto, ResendVerification useCase)
    {
        await useCase.Execute(dto);

        return TypedResults.Ok();
    }

    public async Task<IResult> VerifyEmailChange(string token, VerifyEmailChange useCase)
    {
        try
        {
            await useCase.Execute(token);

            return Results.Redirect($"{this._frontendUrl}/email-verified?status=email-changed");
        }
        catch (ApiException)
        {
            return Results.Redirect($"{this._frontendUrl}/email-verified?status=invalid");
        }
    }
}
