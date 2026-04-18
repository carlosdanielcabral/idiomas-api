using System.ComponentModel.DataAnnotations;

namespace Idiomas.Core.Presentation.DTO.Conversation;

public class SendMessageRequestDTO
{
    [Required]
    [StringLength(4000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}
