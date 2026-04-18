using Idiomas.Core.Domain.Entity;
using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Infrastructure.Database.Mapper;
using Idiomas.Core.Infrastructure.Database.Model;
using Idiomas.Core.Interface.Repository;
using Idiomas.Core.Interface.Service;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Repository;

public class ConversationRepository(
    ApplicationContext database,
    IEncryptionService encryptionService) : IConversationRepository
{
    private readonly ApplicationContext _database = database;
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task<Conversation> Insert(Conversation conversation)
    {
        await this._database.Conversation.AddAsync(conversation.ToModel());
        await this._database.SaveChangesAsync();

        return conversation;
    }

    public async Task<Conversation?> GetById(string id)
    {
        Guid conversationId = Guid.Parse(id);

        ConversationModel? model = await this._database.Conversation
            .Include(c => c.Messages)
            .ThenInclude(m => m.Corrections)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        return model?.ToEntity(this._encryptionService);
    }

    public async Task<IEnumerable<Conversation>> GetByUserId(string userId)
    {
        Guid userGuid = Guid.Parse(userId);

        List<ConversationModel> models = await this._database.Conversation
            .Include(c => c.Messages)
            .Where(c => c.UserId == userGuid)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();

        return models.ToEntities(this._encryptionService);
    }

    public async Task Inactivate(string id)
    {
        Guid conversationId = Guid.Parse(id);

        ConversationModel? model = await this._database.Conversation
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (model == null)
        {
            throw new KeyNotFoundException($"Conversation with ID {conversationId} not found.");
        }

        model.IsActive = false;
        model.UpdatedAt = DateTime.UtcNow;

        await this._database.SaveChangesAsync();
    }

    public async Task<Message> InsertMessage(Message message)
    {
        await this._database.Message.AddAsync(message.ToModel(this._encryptionService));

        ConversationModel? conversation = await this._database.Conversation
            .FirstOrDefaultAsync(c => c.Id == Guid.Parse(message.ConversationId));

        if (conversation != null)
        {
            conversation.UpdatedAt = DateTime.UtcNow;
        }

        await this._database.SaveChangesAsync();

        return message;
    }

    public async Task<Correction> InsertCorrection(Correction correction)
    {
        await this._database.Correction.AddAsync(correction.ToModel(this._encryptionService));
        await this._database.SaveChangesAsync();

        return correction;
    }
}
