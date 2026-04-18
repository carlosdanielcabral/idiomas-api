using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idiomas.Core.Infrastructure.Database.Model;

[Table("message")]
public class MessageModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [Column("conversation_id")]
    public required Guid ConversationId { get; set; }

    [Required]
    [Column("role")]
    [MaxLength(50)]
    public required string Role { get; set; }

    [Required]
    [Column("content")]
    public required string Content { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ConversationId")]
    public ConversationModel? Conversation { get; set; }

    public ICollection<CorrectionModel> Corrections { get; set; } = [];
}
