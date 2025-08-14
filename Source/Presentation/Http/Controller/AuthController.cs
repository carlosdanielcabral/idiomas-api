using Idiomas.Source.Application.DTO.Auth;
using Idiomas.Source.Application.UseCase.AuthCase;
using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Interface.Controller;
using Idiomas.Source.Interface.Service;
using Idiomas.Source.Presentation.DTO.Auth;
using Idiomas.Source.Presentation.Mapper;

namespace Idiomas.Source.Presentation.Http.Controller;

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