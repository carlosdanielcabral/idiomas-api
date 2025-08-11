using IdiomasAPI.Source.Domain.Entity;
using IdiomasAPI.Source.Infrastructure.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace IdiomasAPI.Source.Infrastructure.Database.Context;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<UserModel> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.HasKey(e => e.Id);

            entity.Property(p => p.Id)
                    .HasColumnName("user")
                    .ValueGeneratedNever();

            entity.Property(p => p.Name)
                    .HasColumnName("name")
                    .HasMaxLength(150)
                    .IsRequired();

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(p => p.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsRequired();
                  
            entity.Property(p => p.Password)
                  .HasColumnName("password");
        });
    }
}