using IdiomasAPI.Source.Application.DTO.Auth;
using IdiomasAPI.Source.Application.UseCase.AuthCase;
using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Interface.Controller;
using IdiomasAPI.Source.Interface.Service;
using IdiomasAPI.Source.Presentation.DTO.Auth;
using IdiomasAPI.Source.Presentation.Mapper;

namespace IdiomasAPI.Source.Presentation.Http.Controller;

public class AuthController(MailPasswordLogin mailPasswordUseCase, IToken tokenGenerator) : IAuthController
{
    private readonly MailPasswordLogin _mailPasswordUseCase = mailPasswordUseCase;
    private readonly IToken _tokenGenerator = tokenGenerator;

    public async Task<IResult> MailPasswordLogin(MailPasswordLoginDTO dto)
    {
        User user = await this._mailPasswordUseCase.Execute(dto);

        MailPasswordLoginResponseDTO response = new()
        {
            User = user.ToResponseDTO(),
            Token = this._tokenGenerator.Generate(user)
        };

        return TypedResults.Ok(response);
    }
}