using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idiomas.Core.Infrastructure.Database.Model;

[Table("scenario")]
public class ScenarioModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [Column("language")]
    [MaxLength(50)]
    public required string Language { get; set; }

    [Required]
    [Column("title")]
    [MaxLength(255)]
    public required string Title { get; set; }

    [Required]
    [Column("description")]
    public required string Description { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public ICollection<ConversationModel> Conversations { get; set; } = [];
}
