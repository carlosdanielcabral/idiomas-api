using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class EmailVerificationTokenRepository(ApplicationContext database) : IEmailVerificationTokenRepository
{
    private readonly ApplicationContext _database = database;

    public async Task Insert(EmailVerificationToken token)
    {
        EmailVerificationTokenModel model = token.ToModel();

        this._database.EmailVerificationToken.Add(model);

        await this._database.SaveChangesAsync();
    }

    public async Task<EmailVerificationToken?> GetByTokenHash(string tokenHash)
    {
        EmailVerificationTokenModel? model = await this._database.EmailVerificationToken
            .FirstOrDefaultAsync(record => record.TokenHash == tokenHash);

        return model?.ToEntity();
    }

    public async Task<EmailVerificationToken?> GetActiveTokenByUserId(Guid userId)
    {
        EmailVerificationTokenModel? model = await this._database.EmailVerificationToken
            .Where(record => record.UserId == userId && record.UsedAt == null && record.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(record => record.CreatedAt)
            .FirstOrDefaultAsync();

        return model?.ToEntity();
    }

    public async Task MarkAsUsed(EmailVerificationToken token)
    {
        EmailVerificationTokenModel? model = await this._database.EmailVerificationToken
            .FirstOrDefaultAsync(record => record.Id == token.Id);

        if (model is null)
        {
            throw new KeyNotFoundException($"Email verification token with ID {token.Id} not found.");
        }

        model.UsedAt = DateTime.UtcNow;

        await this._database.SaveChangesAsync();
    }
}
