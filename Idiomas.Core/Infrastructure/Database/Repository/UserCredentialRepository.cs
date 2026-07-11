using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Domain.Enum;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class UserCredentialRepository(ApplicationContext database) : IUserCredentialRepository
{
    private readonly ApplicationContext _database = database;

    public async Task<UserCredential> Insert(UserCredential credential)
    {
        this._database.UserCredential.Add(credential.ToModel());

        await this._database.SaveChangesAsync();

        return credential;
    }

    public async Task<UserCredential?> GetByExternalSubject(AuthProvider provider, string externalSubject)
    {
        UserCredentialModel? model = await this._database.UserCredential
            .FirstOrDefaultAsync(credential => credential.Provider == provider && credential.ExternalSubject == externalSubject);

        return model?.ToEntity();
    }

    public async Task<UserCredential?> GetByUserIdAndProvider(string userId, AuthProvider provider)
    {
        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            return null;
        }

        UserCredentialModel? model = await this._database.UserCredential
            .FirstOrDefaultAsync(credential => credential.UserId == userGuid && credential.Provider == provider);

        return model?.ToEntity();
    }

    public async Task<UserCredential> Update(UserCredential credential)
    {
        Guid credentialId = Guid.Parse(credential.Id);

        UserCredentialModel? outdatedCredential = await this._database.UserCredential
            .FirstOrDefaultAsync(credentialModel => credentialModel.Id == credentialId);

        if (outdatedCredential is null)
        {
            throw new KeyNotFoundException($"Credential with ID {credentialId} not found.");
        }

        outdatedCredential.PasswordHash = credential.PasswordHash;
        outdatedCredential.ExternalSubject = credential.ExternalSubject;

        await this._database.SaveChangesAsync();

        return outdatedCredential.ToEntity();
    }
}
