using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Interface.Repository;
using IdiomasAPI.Source.Interface.Service;
using IdiomasAPI.Source.Application.Error;
using System.Net;
using IdiomasAPI.Source.Application.DTO.Auth;

namespace IdiomasAPI.Source.Application.UseCase.AuthCase;

public class MailPasswordLogin(IUserRepository userRepository, IHash hash)
{
    private IUserRepository _userRepository = userRepository;
    private IHash _hash = hash;

    public async Task<User> Execute(MailPasswordLoginDTO dto)
    {
        User? previousUser = await this._userRepository.GetByEmail(dto.Email);

        if (previousUser == null)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }

        bool isPasswordValid = this._hash.Verify(dto.Password, previousUser.Password);

        if (!isPasswordValid)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }

        return previousUser;
    }
}