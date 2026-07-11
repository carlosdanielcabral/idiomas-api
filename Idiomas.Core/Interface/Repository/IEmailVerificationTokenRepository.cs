using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Interface.Repository;

public interface IEmailVerificationTokenRepository
{
    Task Insert(EmailVerificationToken token);

    Task<EmailVerificationToken?> GetByTokenHash(string tokenHash);

    Task<EmailVerificationToken?> GetActiveTokenByUserId(Guid userId);

    Task MarkAsUsed(EmailVerificationToken token);
}
