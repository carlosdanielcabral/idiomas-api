using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Idiomas.Source.Domain.Entity;
using Idiomas.Source.Domain.Enum;

namespace Idiomas.Source.Infrastructure.Database.Model;

[Table("file")]
public class FileModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] 
    public required Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("original_name")]
    public required string OriginalName { get; set; }

    [Required]
    [MaxLength(52)]
    [Column("key")]
    public required string Key { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("mime_type")]
    public required string MimeType { get; set; }

    [Required]
    [Column("size")]
    public required long Size { get; set; }

    [Required]
    [Column("status")]
    public required FileStatus Status { get; set; } = FileStatus.Pending;

    [Required]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    public UserModel? User { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}