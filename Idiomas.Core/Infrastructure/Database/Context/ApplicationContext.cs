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
    public DbSet<PasswordResetTokenModel> PasswordResetToken { get; set; }
    public DbSet<UserCredentialModel> UserCredential { get; set; }
    public DbSet<EmailVerificationTokenModel> EmailVerificationToken { get; set; }
    public DbSet<EmailChangeRequestModel> EmailChangeRequest { get; set; }

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

        modelBuilder.Entity<PasswordResetTokenModel>()
            .HasIndex(token => token.Token)
            .IsUnique();

        modelBuilder.Entity<PasswordResetTokenModel>()
            .HasIndex(token => token.UserId);

        modelBuilder.Entity<UserCredentialModel>()
            .HasIndex(credential => new { credential.Provider, credential.ExternalSubject })
            .IsUnique();

        modelBuilder.Entity<UserCredentialModel>()
            .HasIndex(credential => new { credential.UserId, credential.Provider })
            .IsUnique();

        modelBuilder.Entity<UserCredentialModel>()
            .HasOne(credential => credential.User)
            .WithMany()
            .HasForeignKey(credential => credential.UserId);

        modelBuilder.Entity<UserModel>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<EmailVerificationTokenModel>()
            .HasIndex(token => token.TokenHash)
            .IsUnique();

        modelBuilder.Entity<EmailVerificationTokenModel>()
            .HasIndex(token => token.UserId);

        modelBuilder.Entity<EmailVerificationTokenModel>()
            .HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId);

        modelBuilder.Entity<EmailChangeRequestModel>()
            .HasIndex(request => request.TokenHash)
            .IsUnique();

        modelBuilder.Entity<EmailChangeRequestModel>()
            .HasIndex(request => request.UserId);

        modelBuilder.Entity<EmailChangeRequestModel>()
            .HasOne(request => request.User)
            .WithMany()
            .HasForeignKey(request => request.UserId);
    }
}