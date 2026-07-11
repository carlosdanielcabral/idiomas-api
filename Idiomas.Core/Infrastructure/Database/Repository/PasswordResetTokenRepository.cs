using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class PasswordResetTokenRepository(ApplicationContext database) : IPasswordResetTokenRepository
{
    private readonly ApplicationContext _database = database;

    public async Task Insert(PasswordResetToken token)
    {
        PasswordResetTokenModel model = token.ToModel();

        this._database.PasswordResetToken.Add(model);

        await this._database.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetByToken(string token)
    {
        PasswordResetTokenModel? model = await this._database.PasswordResetToken
            .FirstOrDefaultAsync(record => record.Token == token);

        return model?.ToEntity();
    }

    public async Task<PasswordResetToken?> GetActiveTokenByUserId(Guid userId)
    {
        PasswordResetTokenModel? model = await this._database.PasswordResetToken
            .Where(record => record.UserId == userId && record.UsedAt == null && record.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(record => record.CreatedAt)
            .FirstOrDefaultAsync();

        return model?.ToEntity();
    }

    public async Task MarkAsUsed(PasswordResetToken token)
    {
        PasswordResetTokenModel? model = await this._database.PasswordResetToken
            .FirstOrDefaultAsync(record => record.Id == token.Id);

        if (model is null)
        {
            throw new KeyNotFoundException($"Password reset token with ID {token.Id} not found.");
        }

        model.UsedAt = DateTime.UtcNow;

        await this._database.SaveChangesAsync();
    }
}
