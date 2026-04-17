using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idiomas.Core.Infrastructure.Database.Model;

[Table("correction")]
public class CorrectionModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [Column("message_id")]
    public required Guid MessageId { get; set; }

    [Required]
    [Column("original_fragment")]
    public required string OriginalFragment { get; set; }

    [Required]
    [Column("suggested_fragment")]
    public required string SuggestedFragment { get; set; }

    [Required]
    [Column("explanation")]
    public required string Explanation { get; set; }

    [Required]
    [Column("type")]
    [MaxLength(50)]
    public required string Type { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("MessageId")]
    public MessageModel? Message { get; set; }
}
