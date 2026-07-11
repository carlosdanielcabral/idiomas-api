using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idiomas.Core.Infrastructure.Database.Model;

[Table("email_verification_token")]
public class EmailVerificationTokenModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    [Required]
    [MaxLength(128)]
    [Column("token_hash")]
    public required string TokenHash { get; set; }

    [Required]
    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }

    [Required]
    [Column("expires_at")]
    public required DateTime ExpiresAt { get; set; }

    [Column("used_at")]
    public DateTime? UsedAt { get; set; }

    [ForeignKey("UserId")]
    public UserModel? User { get; set; }
}
