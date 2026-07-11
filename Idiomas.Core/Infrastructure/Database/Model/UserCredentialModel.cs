using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Idiomas.Core.Domain.Enum;

namespace Idiomas.Core.Infrastructure.Database.Model;

[Table("user_credential")]
public class UserCredentialModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    [Required]
    [Column("provider")]
    public required AuthProvider Provider { get; set; }

    [MaxLength(255)]
    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [MaxLength(255)]
    [Column("external_subject")]
    public string? ExternalSubject { get; set; }

    [Required]
    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }

    public UserModel? User { get; set; }
}
