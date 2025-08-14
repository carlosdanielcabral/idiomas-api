using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.UseCase.AuthCase;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Service;
using Idiomas.Api.Interface.Controller;
using Idiomas.Api.DTO.Auth;
using Idiomas.Api.Mapper;

namespace Idiomas.Api.Http.Controller;

public class AuthController(IToken tokenGenerator) : IAuthController
{
    private readonly IToken _tokenGenerator = tokenGenerator;

    public async Task<IResult> MailPasswordLogin(MailPasswordLoginDTO dto, MailPasswordLogin useCase)
    {
        User user = await useCase.Execute(dto);

        MailPasswordLoginResponseDTO response = new()
        {
            User = user.ToResponseDTO(),
            Token = this._tokenGenerator.Generate(user)
        };

        return TypedResults.Ok(response);
    }
}