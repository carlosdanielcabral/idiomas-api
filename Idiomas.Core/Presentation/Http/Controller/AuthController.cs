using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Controller;
using Idiomas.Core.Interface.Service;
using Idiomas.Core.Presentation.DTO.Auth;
using Idiomas.Core.Presentation.Mapper;

namespace Idiomas.Core.Presentation.Http.Controller;

public class AuthController(IToken tokenGenerator) : IAuthController
{
    private readonly IToken _tokenGenerator = tokenGenerator;

    public async Task<IResult> MailPasswordLogin(HttpContext httpContext, MailPasswordLoginDTO dto, MailPasswordLogin useCase)
    {
        User user = await useCase.Execute(dto);

        MailPasswordLoginResponseDTO response = new()
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
}