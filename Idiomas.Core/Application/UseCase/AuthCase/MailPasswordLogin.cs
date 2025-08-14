using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Idiomas.Core.Application.Error;
using System.Net;
using Idiomas.Core.Application.DTO.Auth;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class MailPasswordLogin(IUserRepository userRepository, IHash hash)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHash _hash = hash;

    public async Task<User> Execute(MailPasswordLoginDTO dto)
    {
        User? user = await this._userRepository.GetByEmail(dto.Email);

        this.ValidateLogin(user, dto);

        return user!;
    }

    public void ValidateLogin(User? user, MailPasswordLoginDTO dto)
    {
        if (user == null)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }

        bool isPasswordValid = this._hash.Verify(dto.Password, user.Password);

        if (!isPasswordValid)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }

    }
}