using Idiomas.Core.Application.DTO.Auth;
using Idiomas.Core.Application.Error;
using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using System.Net;

namespace Idiomas.Core.Application.UseCase.AuthCase;

public class MailPasswordLogin(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IHash _hash = hash;

    public async Task<User> Execute(MailPasswordLoginDTO dto)
    {
        User? user = await this._userRepository.GetByEmail(dto.Email);

        if (user == null)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }

        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(user.Id, AuthProvider.Local);

        if (credential == null)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }

        bool isPasswordValid = this._hash.Verify(dto.Password, credential.PasswordHash!);

        if (!isPasswordValid)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }

        return user;
    }
}
