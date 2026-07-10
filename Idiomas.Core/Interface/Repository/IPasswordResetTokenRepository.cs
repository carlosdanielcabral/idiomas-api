using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Interface.Repository;

public interface IPasswordResetTokenRepository
{
    public Task Insert(PasswordResetToken token);

    public Task<PasswordResetToken?> GetByToken(string token);

    public Task<PasswordResetToken?> GetActiveTokenByUserId(Guid userId);

    public Task MarkAsUsed(PasswordResetToken token);
}
