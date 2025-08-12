using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdiomasAPI.Source.Infrastructure.Database.Model;

[Table("meaning")]
public class MeaningModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] 
    public required Guid Id { get; set; }

    [Required]
    [MaxLength(512)]
    [Column("meaning")]
    public required string Meaning { get; set; }

    [MaxLength(512)]
    [Column("example")]
    public string? Example { get; set; }

    [Required]
    [Column("word_id")]
    public Guid? WordId { get; set; }

    [ForeignKey("WordId")]
    public WordModel? Word { get; set; }
}