using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idiomas.Source.Infrastructure.Database.Model;

[Table("word")]
public class WordModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] 
    public required Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("word")]
    public required string Word { get; set; }

    [Required]
    [MaxLength(128)]
    [Column("ipa")]
    public required string Ipa { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Required]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public UserModel? User { get; set; }

    public ICollection<MeaningModel> Meanings { get; set; } = [];
}