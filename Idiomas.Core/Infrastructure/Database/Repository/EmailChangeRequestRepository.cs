using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class EmailChangeRequestRepository(ApplicationContext database) : IEmailChangeRequestRepository
{
    private readonly ApplicationContext _database = database;

    public async Task Insert(EmailChangeRequest request)
    {
        EmailChangeRequestModel model = request.ToModel();

        this._database.EmailChangeRequest.Add(model);

        await this._database.SaveChangesAsync();
    }

    public async Task<EmailChangeRequest?> GetByTokenHash(string tokenHash)
    {
        EmailChangeRequestModel? model = await this._database.EmailChangeRequest
            .FirstOrDefaultAsync(record => record.TokenHash == tokenHash);

        return model?.ToEntity();
    }

    public async Task<EmailChangeRequest?> GetActiveRequestByUserId(Guid userId)
    {
        EmailChangeRequestModel? model = await this._database.EmailChangeRequest
            .Where(record => record.UserId == userId && record.UsedAt == null && record.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(record => record.CreatedAt)
            .FirstOrDefaultAsync();

        return model?.ToEntity();
    }

    public async Task MarkAsUsed(EmailChangeRequest request)
    {
        EmailChangeRequestModel? model = await this._database.EmailChangeRequest
            .FirstOrDefaultAsync(record => record.Id == request.Id);

        if (model is null)
        {
            throw new KeyNotFoundException($"Email change request with ID {request.Id} not found.");
        }

        model.UsedAt = DateTime.UtcNow;

        await this._database.SaveChangesAsync();
    }
}
