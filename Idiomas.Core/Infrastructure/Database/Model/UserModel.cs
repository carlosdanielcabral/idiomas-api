using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idiomas.Core.Infrastructure.Database.Model;

[Table("user")]
public class UserModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("name")]
    public required string Name { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public required string Email { get; set; }

    [Required]
    [Column("is_email_verified")]
    public required bool IsEmailVerified { get; set; }

    public ICollection<WordModel> Dictionary { get; set; } = [];
    public ICollection<FileModel> Files { get; set; } = [];
}
