using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Interface.Repository;

public interface IUserCredentialRepository
{
    public Task<UserCredential> Insert(UserCredential credential);

    public Task<UserCredential?> GetByExternalSubject(AuthProvider provider, string externalSubject);

    public Task<UserCredential?> GetByUserIdAndProvider(string userId, AuthProvider provider);

    public Task<UserCredential> Update(UserCredential credential);
}
