using Idiomas.Core.Infrastructure.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Idiomas.Core.Infrastructure.Database.Context;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<UserModel> User { get; set; }
    public DbSet<WordModel> Word { get; set; }
    public DbSet<MeaningModel> Meaning { get; set; }
    public DbSet<FileModel> File { get; set; }
    public DbSet<ConversationModel> Conversation { get; set; }
    public DbSet<MessageModel> Message { get; set; }
    public DbSet<CorrectionModel> Correction { get; set; }
    public DbSet<ScenarioModel> Scenario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ConversationModel>()
            .HasIndex(c => c.UserId);

        modelBuilder.Entity<ConversationModel>()
            .HasIndex(c => c.IsActive);

        modelBuilder.Entity<MessageModel>()
            .HasIndex(m => m.ConversationId);

        modelBuilder.Entity<MessageModel>()
            .HasIndex(m => new { m.ConversationId, m.CreatedAt });

        modelBuilder.Entity<CorrectionModel>()
            .HasIndex(c => c.MessageId);

        modelBuilder.Entity<ScenarioModel>()
            .HasIndex(s => s.Language);
    }
}