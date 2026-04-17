using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idiomas.Core.Infrastructure.Database.Model;

[Table("conversation")]
public class ConversationModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    [Required]
    [Column("language")]
    [MaxLength(50)]
    public required string Language { get; set; }

    [Required]
    [Column("mode")]
    [MaxLength(50)]
    public required string Mode { get; set; }

    [Column("scenario_id")]
    public Guid? ScenarioId { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey("UserId")]
    public UserModel? User { get; set; }

    [ForeignKey("ScenarioId")]
    public ScenarioModel? Scenario { get; set; }

    public ICollection<MessageModel> Messages { get; set; } = [];
}
