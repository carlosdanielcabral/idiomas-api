using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdiomasAPI.Source.Infrastructure.Database.Model;

[Table("meaning")]
public class MeaningModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(512)]
    [Column("meaning")]
    public required string Meaning { get; set; }

    [MaxLength(512)]
    [Column("example")]
    public string? Example { get; set; }

    [Required]
    [Column("word_id")]
    public int WordId { get; set; }

    [ForeignKey("WordId")]
    public required WordModel Word { get; set; }
}