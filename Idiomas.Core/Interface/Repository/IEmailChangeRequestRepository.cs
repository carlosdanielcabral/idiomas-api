using Idiomas.Core.Domain.Entity;

namespace Idiomas.Core.Interface.Repository;

public interface IEmailChangeRequestRepository
{
    Task Insert(EmailChangeRequest request);

    Task<EmailChangeRequest?> GetByTokenHash(string tokenHash);

    Task<EmailChangeRequest?> GetActiveRequestByUserId(Guid userId);

    Task MarkAsUsed(EmailChangeRequest request);
}
